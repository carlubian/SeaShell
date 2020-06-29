using Pastel;
using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
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
            if (And(ParamHasValue("_default"), ParamNotExists("Command")).Eval(parameters))
            {
                parameters.TryGetValue("_default", out cmdName);
            }

            // Command name in /Command parameter
            if (And(ParamIsEmpty("_default"), ParamExists("Command"), ParamHasValue("Command")).Eval(parameters))
            {
                parameters.TryGetValue("Command", out cmdName);
            }

            // No default value and Command parameter missing
            if (And(ParamIsEmpty("_default"), ParamNotExists("Command")).Eval(parameters))
            {
                SeaShellErrors.NotifyMissingParam("Command");
                return Enumerable.Empty<dynamic>();
            }

            // Command parameter is empty
            if (And(ParamIsEmpty("_default"), ParamExists("Command"), ParamIsEmpty("Command")).Eval(parameters))
            {
                SeaShellErrors.NotifyParamMissingValue("Command");
                return Enumerable.Empty<dynamic>();
            }

            // Default parameter has value and Command parameter present
            if (And(ParamHasValue("_default"), ParamExists("Command"), ParamHasValue("Command")).Eval(parameters))
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Command");
                return Enumerable.Empty<dynamic>();
            }

            var cmd = Commands.HandlerFor(cmdName);

            // Command doesn't exist
            if (cmd is null)
            {
                ConsoleIO.WriteError($"The command {cmdName} is not recognized.");
                return Enumerable.Empty<dynamic>();
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

            return Enumerable.Empty<dynamic>();
        }
    }
}
