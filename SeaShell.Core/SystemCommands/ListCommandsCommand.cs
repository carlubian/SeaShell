using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.SystemCommands
{
    public class ListCommandsCommand : ISeaShellCommand
    {
        public string Name => "List-Commands";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Show a list of all available commands.",
            Example = "List-Commands"
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            foreach (var cmd in Commands.AllCommands)
            {
                ConsoleIO.WriteInfo(cmd.Key);
                Console.WriteLine($"  {cmd.Value.Help.Description}");
            }

            return null;
        }
    }
}
