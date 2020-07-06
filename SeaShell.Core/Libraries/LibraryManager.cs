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
                if (!(Activator.CreateInstance(cmd) is ISeaShellCommand command))
                    continue;

                Commands.AllCommands.Add(command.Name, command);
                (Commands.CommandsPerLibrary[libName] as List<string>)?.Add(command.Name);
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
                if (!(Activator.CreateInstance(cmd) is ISeaShellCommand command))
                    continue;

                Commands.LocalCommands.Add(command.Name, command);
                (Commands.LocalCommandsPerLibrary[libName] as List<string>)?.Add(command.Name);
            }
        }
    }
}
