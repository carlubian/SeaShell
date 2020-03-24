﻿using Ionic.Zip;
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

        private static void LoadAssembly(string asm)
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
