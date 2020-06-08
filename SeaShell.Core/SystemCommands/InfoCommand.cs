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
    public class InfoCommand : ISeaShellCommand
    {
        public string Name => "Info";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Shows SeaShell environment information.",
            Example = "Info [/Version /OS /All]",
            Parameters = new Dictionary<string, string>
            {
                { "/Version", "Display the current SeaShell interpreter version." },
                { "/OS", "Display the current Operating System version." },
                { "/All", "Display all available environment information." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            // All parameter and other parameters present
            if (Parameters.SeeIf(parameters).HasParam("All").HasParam("Version").Eval()
                || Parameters.SeeIf(parameters).HasParam("All").HasParam("OS").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("All", "Version", "OS");
                return null;
            }

            // No parameters present
            if (Parameters.SeeIf(parameters).HasNone("All").HasNone("Version")
                .HasNone("OS").Eval())
            {
                SeaShellErrors.NotifyMissingOneOfParams("All", "Version", "OS");
                return null;
            }

            // All parameter present
            if (Parameters.SeeIf(parameters).HasParam("All").Eval())
            {
                PrintVersion();
                PrintOS();
            }

            // Version parameter present
            if (Parameters.SeeIf(parameters).HasParam("Version").Eval())
                PrintVersion();

            // Version parameter present
            if (Parameters.SeeIf(parameters).HasParam("OS").Eval())
                PrintOS();

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
