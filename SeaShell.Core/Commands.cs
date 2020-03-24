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

        internal static void PopulateSystemCommands()
        {
            AllCommands.Add("Environment", new EnvironmentCommand());
            AllCommands.Add("Exit", new ExitCommand());
            AllCommands.Add("Help", new HelpCommand());
            AllCommands.Add("List-Commands", new ListCommandsCommand());
        }

        internal static void PopulateGlobalCommands()
        {
            // Temp
            //LibraryManager.InstallGlobalLibrary(@"C:\Users\carlu\Downloads\SeaShell.IO.ssl");
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
