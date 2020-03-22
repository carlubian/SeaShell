using ConfigAdapter.Xml;
using SeaShell.Core.Extensibility;
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
        }

        internal static void PopulateGlobalCommands()
        {
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell");
            //XmlConfig.From(Path.Combine(directory, "Configuration.xml"));

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
