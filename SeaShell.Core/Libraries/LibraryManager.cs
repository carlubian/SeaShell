using ConfigAdapter.Ini;
using DotNet.Misc.Extensions.Linq;
using Ionic.Zip;
using SeaShell.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;

namespace SeaShell.Core.Libraries
{
    internal static class LibraryManager
    {
        internal static IDictionary<string, AssemblyLoadContext> Libraries = new Dictionary<string, AssemblyLoadContext>();

        internal static void InstallGlobalLibrary(string path)
        {
            // Libraries go in %UserProfile%\.seashell\Libraries
            var LibDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "Libraries");
            var TmpDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "temp");
            //CleanDirectory(TmpDir);

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
            var loadContext = new WeakReference<AssemblyLoadContext>(Libraries[name]);
            Libraries.Remove(name);
            loadContext.TryGetTarget(out var lc);

            // Unregister all commands in the assembly
            foreach (var cmd in Commands.CommandsPerLibrary[name])
            {
                Commands.AllCommands.Remove(cmd);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

            lc.Unloading += lc =>
            {
                var LibDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "Libraries");
                foreach (var directory in Directory.EnumerateDirectories(LibDir))
                    if (new DirectoryInfo(directory).Name.Equals(name))
                    {
                        CleanDirectory(Path.Combine(LibDir, directory));
                        Directory.Delete(Path.Combine(LibDir, directory));
                        break;
                    }
            };
            lc.Unload();
            
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
    }
}
