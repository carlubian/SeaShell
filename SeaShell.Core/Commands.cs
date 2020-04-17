using ConfigAdapter.Xml;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Libraries;
using SeaShell.Core.SystemCommands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SeaShell.Core
{
    internal static class Commands
    {
        internal static IDictionary<string, ISeaShellCommand> AllCommands = new Dictionary<string, ISeaShellCommand>();
        internal static IDictionary<string, IEnumerable<string>> CommandsPerLibrary = new Dictionary<string, IEnumerable<string>>();

        internal static void PopulateSystemCommands()
        {
            AllCommands.Add("Environment", new EnvironmentCommand());
            AllCommands.Add("Exit", new ExitCommand());
            AllCommands.Add("Help", new HelpCommand());
            AllCommands.Add("List-Commands", new ListCommandsCommand());
            AllCommands.Add("Print", new PrintCommand());
            AllCommands.Add("Change-Directory", new ChangeDirectoryCommand());
            AllCommands.Add("Otter", new OtterCommand());
        }

        internal static void PopulateGlobalCommands()
        {
            var LibDir = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell"), "Libraries");

            foreach (var library in Directory.EnumerateDirectories(LibDir))
            {
                var manifest = Manifest.Parse(Path.Combine(library, "manifest.ini"));
                if (manifest is null)
                    continue;

                var AsmDir = Path.Combine(library, "Assemblies");

                // Load assemblies
                foreach (var asm in manifest.Assemblies)
                    LibraryManager.LoadAssembly(Path.Combine(AsmDir, asm), manifest.Name);
            }
        }

        internal static void PopulateLocalCommands()
        {

        }

        internal static ISeaShellCommand HandlerFor(string name)
        {
            if (AllCommands.ContainsKey(name))
                return AllCommands[name];

            return null;
        }
    }
}
