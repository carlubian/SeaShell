using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using System.Collections.Generic;
using System.Linq;

namespace SeaShell.Core.SystemCommands
{
    public class ExitCommand : ISeaShellCommand
    {
        public string Name => "Exit";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Terminates the current SeaShell session.",
            Example = "Exit"
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (parameters.Any(p => p.Key != "_default"))
                ConsoleIO.WriteWarning("The Exit command should not have any parameters.");
            if (pipeline != null)
                ConsoleIO.WriteWarning("The Exit command should not be part of a pipeline.");

            SeaShellHost.Continue = false;
            return Enumerable.Empty<dynamic>();
        }
    }
}
