using ConfigAdapter.Ini;
using Pastel;
using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
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
            if (And(Or(ParamExists("Pack"), ParamExists("Unpack"), ParamExists("Install"), ParamExists("List"), ParamExists("Remove"), ParamExists("Create")),
                MutuallyExclusive("Pack", "Unpack", "Install", "List", "Remove", "Create")).Eval(parameters))
            {
                // Pack
                if (parameters.TryGetValue("Pack", out var PackValue))
                {
                    var dirName = Environment.CurrentDirectory;

                    if (PackValue != "")
                        dirName = PackValue;

                    // TODO check for errors (missing directory, incorrect format)
                    LibraryManager.Pack(dirName);
                    ConsoleIO.WriteInfo($"Packed library from {dirName}");
                    return Enumerable.Empty<dynamic>();
                }

                // Unpack
                if (parameters.TryGetValue("Unpack", out var UnpackValue))
                {
                    // TODO check for errors (missing file, incorrect format)
                    var library = UnpackValue;
                    LibraryManager.Unpack(library);
                    ConsoleIO.WriteInfo($"Unpacked library file {library}");
                    return Enumerable.Empty<dynamic>();
                }

                // Install
                if (parameters.TryGetValue("Install", out var InstallValue))
                {
                    // TODO check for errors (missing file, incorrect format)
                    LibraryManager.InstallLibrary(InstallValue);
                    return Enumerable.Empty<dynamic>();
                }

                // List
                if (parameters.TryGetValue("List", out var ListValue))
                {
                    var BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell");
                    var LibDir = Path.Combine(BaseDir, "Libraries");

                    // Global commands
                    foreach (var folder in Directory.EnumerateDirectories(LibDir))
                    {
                        var manifest = Directory.EnumerateFiles(folder).FirstOrDefault(f => f.EndsWith("Manifest.ini"));
                        if (manifest is null)
                            continue;

                        var config = IniConfig.From(manifest);
                        ConsoleIO.WriteInfo($"{config.Read("Library:Name")} {config.Read("Library:Version").Pastel("#ECB310")}");
                        Console.WriteLine($"  {config.Read("Library:Description")}");
                    }
                    // Local commands
                    if (SeaShellHost.Env.Equals("_system"))
                        return Enumerable.Empty<dynamic>();

                    LibDir = Path.Combine(SeaShellHost.EnvPath, "SeaShell.Environment");
                    foreach (var folder in Directory.EnumerateDirectories(LibDir))
                    {
                        var manifest = Directory.EnumerateFiles(folder).FirstOrDefault(f => f.EndsWith("Manifest.ini"));
                        if (manifest is null)
                            continue;

                        var config = IniConfig.From(manifest);
                        ConsoleIO.WriteInfo($"{config.Read("Library:Name")} {config.Read("Library:Version").Pastel("#ECB310")}");
                        Console.WriteLine($"  {config.Read("Library:Description")}");
                    }

                    return Enumerable.Empty<dynamic>();
                }

                // Remove
                if (parameters.TryGetValue("Remove", out var RemoveValue))
                {
                    var library = RemoveValue;
                    if (!Commands.CommandsPerLibrary.ContainsKey(library) &&
                        !Commands.LocalCommandsPerLibrary.ContainsKey(library))
                    {
                        ConsoleIO.WriteWarning($"No library named {library} is installed.");
                        return Enumerable.Empty<dynamic>();
                    }

                    LibraryManager.Remove(library);
                    ConsoleIO.WriteInfo($"Removed library {library}");
                    return Enumerable.Empty<dynamic>();
                }

                // Create
                if (parameters.TryGetValue("Create", out var CreateValue))
                {
                    var dirName = Environment.CurrentDirectory;

                    if (CreateValue != "")
                        dirName = CreateValue;

                    // TODO check for existing manifest before overwriting
                    Directory.CreateDirectory(Path.Combine(dirName, "Assemblies"));
                    File.Create(Path.Combine(dirName, "Manifest.ini")).Close();
                    Manifest.WriteTemplateManifest(Path.Combine(dirName, "Manifest.ini"));

                    ConsoleIO.WriteInfo($"Created manifest on {dirName}");
                    return Enumerable.Empty<dynamic>();
                }
            }

            // More than one parameter present
            SeaShellErrors.NotifyMutuallyExclusive("Pack", "Unpack", "Install", "List", "Remove", "Create");
            return Enumerable.Empty<dynamic>();
        }
    }
}
