using Pastel;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.SystemCommands
{
    public class HelpCommand : ISeaShellCommand
    {
        public string Name => "Help";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Shows help information about SeaShell commands.",
            Example = "Help [/Command] Command-Name",
            Parameters = new Dictionary<string, string>
            {
                { "/Command (default)", "Command which help is needed for." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var cmdName = "";

            // Command name in default parameter
            if (Parameters.SeeIf(parameters).HasParam("_default").HasNone("Command")
                .HasValue("_default").Eval())
            {
                cmdName = parameters.Single(p => p.Key == "_default").Value;
            }

            // Command name in /Command parameter
            if (Parameters.SeeIf(parameters).HasParam("Command").HasValue("Command")
                .IsEmpty("_default").Eval())
            {
                cmdName = parameters.Single(p => p.Key == "Command").Value;
            }

            // No default value and Command parameter missing
            if (Parameters.SeeIf(parameters).IsEmpty("_default").HasNone("Command").Eval())
            {
                SeaShellErrors.NotifyMissingParam("Command");
                return null;
            }

            // Command parameter is empty
            if (Parameters.SeeIf(parameters).HasParam("Command").IsEmpty("Command").Eval())
            {
                SeaShellErrors.NotifyParamMissingValue("Command");
                return null;
            }

            // Default parameter has value and Command parameter present
            if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Command")
                .HasValue("Command").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Command");
                return null;
            }

            var cmd = Commands.HandlerFor(cmdName);

            // Command doesn't exist
            if (cmd is null)
            {
                ConsoleIO.WriteError($"The command {cmdName} is not recognized.");
                return null;
            }

            var cmdHelp = cmd.Help;

            Console.WriteLine($"{cmdName.Pastel("#F0F5FF")}: {cmdHelp.Description}".Pastel("#2DA8CA"));
            Console.WriteLine($"Usage: {cmdHelp.Example.Pastel("#F0F5FF")}".Pastel("#2DA8CA"));
            if (cmdHelp.Parameters != null)
            {
                Console.WriteLine("Parameter information:".Pastel("#2DA8CA"));
                foreach (var key in cmdHelp.Parameters.Keys)
                    Console.WriteLine($"  {key}: {cmdHelp.Parameters[key].Pastel("#F0F5FF")}".Pastel("#2DA8CA"));
            }
            else
                Console.WriteLine("This command has no parameters.".Pastel("#2DA8CA"));

            return null;
        }
    }
}
