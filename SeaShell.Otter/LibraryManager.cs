using ConfigAdapter.Ini;
using DotNet.Misc.Extensions.Linq;
using Ionic.Zip;
using SeaShell.Core;
using SeaShell.Core.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace SeaShell.Otter
{
    internal static class LibraryManager
    {
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
                Core.Libraries.LibraryManager.LoadAssembly(Path.Combine(AsmDir, asm), manifest.Name);

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
            var loadContext = new WeakReference<AssemblyLoadContext>(Core.Libraries.LibraryManager.Libraries[name]);
            Core.Libraries.LibraryManager.Libraries.Remove(name);
            loadContext.TryGetTarget(out var lc);

            // Unregister all commands in the assembly
            foreach (var cmd in Commands.CommandsPerLibrary[name])
            {
                Commands.AllCommands.Remove(cmd);
            }
            Commands.CommandsPerLibrary.Remove(name);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            lc?.Unload();

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
            var loadContext = new WeakReference<AssemblyLoadContext>(Core.Libraries.LibraryManager.LocalLibraries[name]);
            Core.Libraries.LibraryManager.LocalLibraries.Remove(name);
            loadContext.TryGetTarget(out var lc);

            // Unregister all commands in the assembly
            foreach (var cmd in Commands.LocalCommandsPerLibrary[name])
            {
                Commands.LocalCommands.Remove(cmd);
            }
            Commands.LocalCommandsPerLibrary.Remove(name);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            lc?.Unload();

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
    }
}
