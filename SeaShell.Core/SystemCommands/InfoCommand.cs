using Pastel;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaShell.Core.SystemCommands
{
    public class InfoCommand : ISeaShellCommand
    {
        public string Name => "Info";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Shows SeaShell environment information.",
            Example = "Info"
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            Console.WriteLine($"{"SeaShell host version:".Pastel("#DCE1EB")} {SeaShellHost.Version.Pastel("#2DA8CA")}");
            Console.WriteLine($"{"Operating System version:".Pastel("#DCE1EB")} {Environment.OSVersion.VersionString.Pastel("#2DA8CA")}");

            return Enumerable.Empty<dynamic>();
        }
    }
}
