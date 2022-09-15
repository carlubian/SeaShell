using Pastel;
using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

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

            if (Or(And(ParamHasValue("_default"), ParamNotExists("Command")), 
                And(ParamIsEmpty("_default"), ParamExists("Command"), ParamHasValue("Command"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out cmdName))
                    parameters.TryGetValue("Command", out cmdName);

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
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
