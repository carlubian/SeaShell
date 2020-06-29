using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Global commands
            foreach (var cmd in Commands.AllCommands)
            {
                ConsoleIO.WriteInfo(cmd.Key);
                Console.WriteLine($"  {cmd.Value.Help.Description}");
            }
            // Local commands
            foreach (var cmd in Commands.LocalCommands)
            {
                ConsoleIO.WriteInfo($"{cmd.Key} [VirtualEnv]");
                Console.WriteLine($"  {cmd.Value.Help.Description}");
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
