using ConfigAdapter.Ini;
using Pastel;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Libraries;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.Core.SystemCommands
{
    public class OtterCommand : ISeaShellCommand
    {
        public string Name => "Otter";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Library manager for SeaShell",
            Example = "Otter [/Pack /Unpack /Install /List /Remove]",
            Parameters = new Dictionary<string, string>
            {
                { "/Pack", "Packs the content of a directory into a SeaShell SSL library." },
                { "/Unpack", "Unpacks a SeaShell SSL library into a subdirectory besides it." },
                { "/Install", "Installs a SeaShell SSL library in the global system catalog." },
                { "/List", "Show all the libraries currently in the global system catalog." },
                { "/Remove", "Packs the content of a directory into a SeaShell SSL library." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            // More than one parameter present
            if (!Parameters.SeeIf(parameters).HasOnlyOne("Pack", "Unpack", "Install", "List", "Remove").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("Pack", "Unpack", "Install", "List", "Remove");
                return null;
            }

            // Pack: Check that directory has proper structure, maybe ignore unneeded files?

            // Unpack
            if (Parameters.SeeIf(parameters).HasParam("Unpack").HasValue("Unpack").Eval())
            {
                // TODO check for errors (missing file, incorrect format)
                var library = parameters.Single(p => p.Key == "Unpack").Value;
                LibraryManager.Unpack(library);
            }

            // Install

            // List
            if (Parameters.SeeIf(parameters).HasParam("List").Eval())
            {
                var BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell");
                var LibDir = Path.Combine(BaseDir, "Libraries");

                foreach (var folder in Directory.EnumerateDirectories(LibDir))
                {
                    var manifest = Directory.EnumerateFiles(folder).FirstOrDefault(f => f.EndsWith("Manifest.ini"));
                    if (manifest is null)
                        continue;

                    var config = IniConfig.From(manifest);
                    ConsoleIO.WriteInfo($"{config.Read("Library:Name")} {config.Read("Library:Version").Pastel("#ECB310")}");
                    Console.WriteLine($"  {config.Read("Library:Description")}");
                }
            }

            // Remove: Requires restart to unload assemblies?

            return null;
        }
    }
}
