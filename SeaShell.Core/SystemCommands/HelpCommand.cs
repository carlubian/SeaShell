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
            if (Parameters.Check(parameters).OnlyOneCanBePresent("_default", "Command").Execute())
            {
                var cmdName = parameters.Single(p => p.Key == "_default").Value;
                if (cmdName is "")
                    cmdName = parameters.Single(p => p.Key == "Command").Value;

                var cmdHelp = Commands.HandlerFor(cmdName).Help;

                Console.WriteLine($"{cmdName.Pastel("#F0F5FF")}: {cmdHelp.Description}".Pastel("#2DA8CA"));
                Console.WriteLine($"Usage: {cmdHelp.Example.Pastel("#F0F5FF")}".Pastel("#2DA8CA"));
                Console.WriteLine("Parameter information:".Pastel("#2DA8CA"));
                foreach (var key in cmdHelp.Parameters.Keys)
                    Console.WriteLine($"  {key}: {cmdHelp.Parameters[key].Pastel("#F0F5FF")}".Pastel("#2DA8CA"));
            }
            
            return null;
        }
    }
}
