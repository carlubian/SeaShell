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
    public class EnvironmentCommand : ISeaShellCommand
    {
        public string Name => "Environment";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Shows SeaShell environment information.",
            Example = "Environment [/Version /OS /All]",
            Parameters = new Dictionary<string, string>
            {
                { "/Version", "Display the current SeaShell interpreter version." },
                { "/OS", "Display the current Operating System version." },
                { "/All", "Display all available environment information." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (Parameters.Check(parameters).MustBePresent("All").Execute())
            {
                PrintVersion();
                PrintOS();
            }
            else
            {
                if (Parameters.Check(parameters).MustBePresent("Version").Execute())
                    PrintVersion();
                if (Parameters.Check(parameters).MustBePresent("OS").Execute())
                    PrintOS();
            }

            void PrintVersion()
            {
                Console.WriteLine($"{"SeaShell host version:".Pastel("#DCE1EB")} {SeaShellHost.Version.Pastel("#2DA8CA")}");
            }
            void PrintOS()
            {
                Console.WriteLine($"{"Operating System version:".Pastel("#DCE1EB")} {Environment.OSVersion.VersionString.Pastel("#2DA8CA")}");
            }

            return null;
        }
    }
}
