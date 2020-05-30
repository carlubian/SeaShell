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
            Example = "Otter [/Pack /Unpack /Install /List /Remove /Create]",
            Parameters = new Dictionary<string, string>
            {
                { "/Pack", "Packs the content of a directory into a SeaShell SSL library." },
                { "/Unpack", "Unpacks a SeaShell SSL library into a subdirectory besides it." },
                { "/Install", "Installs a SeaShell SSL library in the global system catalog." },
                { "/List", "Show all the libraries currently in the global system catalog." },
                { "/Remove", "Packs the content of a directory into a SeaShell SSL library." },
                { "/Create", "Create a manifest for a new library." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            // More than one parameter present
            if (!Parameters.SeeIf(parameters).HasOnlyOne("Pack", "Unpack", "Install", "List", "Remove", "Create").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("Pack", "Unpack", "Install", "List", "Remove", "Create");
                return null;
            }

            // Pack
            if (Parameters.SeeIf(parameters).HasParam("Pack").Eval())
            {
                var dirName = Environment.CurrentDirectory;

                if (parameters.Single(p => p.Key == "Pack").Value != "")
                    dirName = parameters.Single(p => p.Key == "Pack").Value;

                // TODO check for errors (missing directory, incorrect format)
                LibraryManager.Pack(dirName);
                ConsoleIO.WriteInfo($"Packed library from {dirName}");
                return null;
            }

            // Unpack
            if (Parameters.SeeIf(parameters).HasParam("Unpack").HasValue("Unpack").Eval())
            {
                // TODO check for errors (missing file, incorrect format)
                var library = parameters.Single(p => p.Key == "Unpack").Value;
                LibraryManager.Unpack(library);
                ConsoleIO.WriteInfo($"Unpacked library file {library}");
                return null;
            }

            // Install
            if (Parameters.SeeIf(parameters).HasParam("Install").HasValue("Install").Eval())
            {
                // TODO check for errors (missing file, incorrect format)
                var library = parameters.Single(p => p.Key == "Install").Value;
                LibraryManager.InstallGlobalLibrary(library);
                return null;
            }

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
                return null;
            }

            // Remove
            if (Parameters.SeeIf(parameters).HasParam("Remove").HasValue("Remove").Eval())
            {
                var library = parameters.Single(p => p.Key == "Remove").Value;
                if (!Commands.CommandsPerLibrary.ContainsKey(library))
                {
                    ConsoleIO.WriteWarning($"No library named {library} is installed.");
                    return null;
                }

                LibraryManager.Remove(library);
                ConsoleIO.WriteInfo($"Removed library {library}");
                return null;
            }

            // Create
            if (Parameters.SeeIf(parameters).HasParam("Create").Eval())
            {
                var dirName = Environment.CurrentDirectory;

                if (parameters.Single(p => p.Key == "Create").Value != "")
                    dirName = parameters.Single(p => p.Key == "Create").Value;

                // TODO check for existing manifest before overwriting
                Directory.CreateDirectory(Path.Combine(dirName, "Assemblies"));
                File.Create(Path.Combine(dirName, "Manifest.ini")).Close();
                Manifest.WriteTemplateManifest(Path.Combine(dirName, "Manifest.ini"));

                ConsoleIO.WriteInfo($"Created manifest on {dirName}");
                return null;
            }

            // No parameter present
            SeaShellErrors.NotifyMissingOneOfParams("Pack", "Unpack", "Install", "List", "Remove", "Create");
            return null;
        }
    }
}
