using ConfigAdapter.Ini;
using DotNet.Misc.Extensions.Linq;
using Ionic.Zip;
using SeaShell.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace SeaShell.Core.Libraries
{
    internal static class LibraryManager
    {
        internal static IDictionary<string, AssemblyLoadContext> Libraries = new Dictionary<string, AssemblyLoadContext>();
        internal static IDictionary<string, AssemblyLoadContext> LocalLibraries = new Dictionary<string, AssemblyLoadContext>();

        internal static void InstallLibrary(string path)
        {
            string LibDir;
            string TmpDir;
            if (SeaShellHost.Env.Equals("_system"))
            {
                // Global libraries go in %UserProfile%\.seashell\Libraries
                LibDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "Libraries");
                TmpDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "temp");
            }
            else
            {
                // Local libraries go in the environment path
                LibDir = Path.Combine(SeaShellHost.EnvPath, "SeaShell.Environment");
                TmpDir = Path.Combine(SeaShellHost.EnvPath, "SeaShell.Temp");
            }

            using (var Zip = new ZipFile(path))
            {
                Zip.ExtractAll(TmpDir, ExtractExistingFileAction.OverwriteSilently);
            }

            // Open manifest file
            if (!File.Exists(Path.Combine(TmpDir, "manifest.ini")))
            {
                ConsoleIO.WriteError($"Library file {path} has no manifest file.");
                return;
            }
            var manifest = Manifest.Parse(Path.Combine(TmpDir, "manifest.ini"));

            // Copy from temp directory to library directory
            LibDir = Path.Combine(LibDir, manifest.Name);
            if (Directory.Exists(LibDir))
            {
                CleanDirectory(LibDir);
                Directory.Delete(LibDir);
            }
            Directory.Move(TmpDir, LibDir);
            
            var AsmDir = Path.Combine(LibDir, "Assemblies");

            // Load assemblies
            foreach (var asm in manifest.Assemblies)
                LoadAssembly(Path.Combine(AsmDir, asm), manifest.Name);

            ConsoleIO.WriteInfo($"Installed library {manifest.Name}");
        }

        internal static void Unpack(string path)
        {
            var dir = new FileInfo(path).Directory.FullName;
            // Hack to remove file extension.
            var dest = Path.Combine(dir, new FileInfo(path).Name
                .Split('.').Reverse().Skip(1).Reverse().Stringify(n => n, "."));

            if (Directory.Exists(dest))
                CleanDirectory(dest);

            using (var Zip = new ZipFile(path))
            {
                Zip.ExtractAll(dest, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        internal static void Pack(string path)
        {
            var manifest = Directory.EnumerateFiles(path).FirstOrDefault(f => f.EndsWith("Manifest.ini"));
            var config = IniConfig.From(manifest);
            var libName = config.Read("Library:Name");

            if (File.Exists(Path.Combine(path, $"{libName}.ssl")))
                File.Delete(Path.Combine(path, $"{libName}.ssl"));

            using (var Zip = new ZipFile(Path.Combine(path, $"{libName}.ssl")))
            {
                Zip.AddFile(Path.Combine(path, "Manifest.ini"), "");
                Zip.AddDirectory(Path.Combine(path, "Assemblies"), "Assemblies");
                Zip.Save();
            }
        }

        internal static void Remove(string name)
        {
            if (SeaShellHost.Env.Equals("_system"))
                RemoveGlobal(name);
            else
                RemoveLocal(name);
        }

        private static void RemoveGlobal(string name)
        {
            var loadContext = new WeakReference<AssemblyLoadContext>(Libraries[name]);
            Libraries.Remove(name);
            loadContext.TryGetTarget(out var lc);

            // Unregister all commands in the assembly
            foreach (var cmd in Commands.CommandsPerLibrary[name])
            {
                Commands.AllCommands.Remove(cmd);
            }
            Commands.CommandsPerLibrary.Remove(name);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            lc.Unload();

            var LibDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "Libraries");
            foreach (var directory in Directory.EnumerateDirectories(LibDir))
                if (new DirectoryInfo(directory).Name.Equals(name))
                {
                    CleanDirectory(Path.Combine(LibDir, directory));
                    Directory.Delete(Path.Combine(LibDir, directory));
                    break;
                }
        }

        private static void RemoveLocal(string name)
        {
            var loadContext = new WeakReference<AssemblyLoadContext>(LocalLibraries[name]);
            LocalLibraries.Remove(name);
            loadContext.TryGetTarget(out var lc);

            // Unregister all commands in the assembly
            foreach (var cmd in Commands.LocalCommandsPerLibrary[name])
            {
                Commands.LocalCommands.Remove(cmd);
            }
            Commands.LocalCommandsPerLibrary.Remove(name);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            lc.Unload();

            var LibDir = Path.Combine(SeaShellHost.EnvPath, "SeaShell.Environment");
            foreach (var directory in Directory.EnumerateDirectories(LibDir))
                if (new DirectoryInfo(directory).Name.Equals(name))
                {
                    CleanDirectory(Path.Combine(LibDir, directory));
                    Directory.Delete(Path.Combine(LibDir, directory));
                    break;
                }
        }

        private static void CleanDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);

            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (var dir in di.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void LoadAssembly(string asm, string libName)
        {
            var loadContext = new LibraryLoadContext(asm);
            //var lib = Assembly.LoadFrom(asm);
            using var fs = new FileStream(asm, FileMode.Open, FileAccess.Read);
            var lib = loadContext.LoadFromStream(fs);
            Libraries.Add(libName, loadContext);

            // Populate commands
            Commands.CommandsPerLibrary.Add(libName, new List<string>());
            foreach (var cmd in lib.GetTypes().Where(t => typeof(ISeaShellCommand).IsAssignableFrom(t)))
            {
                var command = Activator.CreateInstance(cmd) as ISeaShellCommand;
                Commands.AllCommands.Add(command.Name, command);
                (Commands.CommandsPerLibrary[libName] as List<string>).Add(command.Name);
            }
        }

        /// <summary>
        /// Load a virtual environment from a path.
        /// </summary>
        /// <param name="path"></param>
        internal static void LoadVirtual(string path)
        {
            var fileDir = Path.Combine(path, "SeaShell.Environment.ini");
            var libDir = Path.Combine(path, "SeaShell.Environment");
            var virtualEnv = VirtualEnv.Parse(fileDir);
            
            foreach (var library in Directory.EnumerateDirectories(libDir))
            {
                var libPath = Path.Combine(libDir, library);
                // Open manifest file
                if (!File.Exists(Path.Combine(libPath, "manifest.ini")))
                {
                    ConsoleIO.WriteError($"Library file {path} has no manifest file.");
                    return;
                }
                var manifest = Manifest.Parse(Path.Combine(libPath, "manifest.ini"));
                var AsmDir = Path.Combine(libPath, "Assemblies");

                // Load assemblies
                foreach (var asm in manifest.Assemblies)
                    LoadLocalAssembly(Path.Combine(AsmDir, asm), manifest.Name);
            }
            SeaShellHost.Env = virtualEnv.Name;
            SeaShellHost.EnvPath = path;
        }

        /// <summary>
        /// Unload a virtual environment and return to global.
        /// </summary>
        internal static void UnloadVirtual()
        {
            // TODO check for memory leaks
            LocalLibraries.Clear();
            Commands.LocalCommands.Clear();
            Commands.LocalCommandsPerLibrary.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            SeaShellHost.Env = "_system";
            SeaShellHost.EnvPath = "";
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void LoadLocalAssembly(string asm, string libName)
        {
            var loadContext = new LibraryLoadContext(asm);
            using var fs = new FileStream(asm, FileMode.Open, FileAccess.Read);
            var lib = loadContext.LoadFromStream(fs);
            LocalLibraries.Add(libName, loadContext);

            // Populate commands
            Commands.LocalCommandsPerLibrary.Add(libName, new List<string>());
            foreach (var cmd in lib.GetTypes().Where(t => typeof(ISeaShellCommand).IsAssignableFrom(t)))
            {
                var command = Activator.CreateInstance(cmd) as ISeaShellCommand;
                Commands.LocalCommands.Add(command.Name, command);
                (Commands.LocalCommandsPerLibrary[libName] as List<string>).Add(command.Name);
            }
        }
    }
}
