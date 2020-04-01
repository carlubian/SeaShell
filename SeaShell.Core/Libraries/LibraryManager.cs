using ConfigAdapter.Ini;
using DotNet.Misc.Extensions.Linq;
using Ionic.Zip;
using SeaShell.Core.Extensibility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SeaShell.Core.Libraries
{
    internal static class LibraryManager
    {
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
                LoadAssembly(Path.Combine(AsmDir, asm));
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

            using (var Zip = new ZipFile(Path.Combine(path, $"{libName}.ssl")))
            {
                Zip.AddFile(Path.Combine(path, "Manifest.ini"), "");
                Zip.AddDirectory(Path.Combine(path, "Assemblies"), "Assemblies");
                Zip.Save();
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

        internal static void LoadAssembly(string asm)
        {
            var lib = Assembly.LoadFrom(asm);

            // Populate commands
            foreach (var cmd in lib.GetTypes().Where(t => typeof(ISeaShellCommand).IsAssignableFrom(t)))
            {
                var command = Activator.CreateInstance(cmd) as ISeaShellCommand;
                Commands.AllCommands.Add(command.Name, command);
            }
        }
    }
}
