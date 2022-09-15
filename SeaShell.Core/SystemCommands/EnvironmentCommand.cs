using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Libraries;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeaShell.Core.SystemCommands
{
    public class EnvironmentCommand : ISeaShellCommand
    {
        public string Name => "Environment";

        public CommandHelp Help => new CommandHelp
        {
            Description = "SeaShell virtual environment manager.",
            Example = "Environment [/Load [Path] /Unload /Create [Path] /Info]",
            Parameters = new Dictionary<string, string>
            {
                { "/Load", "Load a virtual environment from either the parameter or the current directory." },
                { "/Unload", "Unload the current virtual environment and return to the base environment." },
                { "/Create", "Create a new virtual environment in either the parameter or the current directory." },
                { "/Info", "Show the currently active virtual environment." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if(And(Or(ParamExists("Load"), ParamExists("Unload"), ParamExists("Create"), ParamExists("Info")), 
                MutuallyExclusive("Load", "Unload", "Create", "Info")).Eval(parameters))
            {
                // Load
                if (parameters.Any(p => p.Key.Equals("Load")))
                {
                    var dirName = Environment.CurrentDirectory;

                    parameters.TryGetValue("Load", out var LoadValue);
                    if (LoadValue != "")
                        dirName = LoadValue;

                    // TODO check for errors (missing directory, incorrect format)
                    LibraryManager.LoadVirtual(dirName);
                    ConsoleIO.WriteInfo($"Loaded virtual environment {SeaShellHost.Env}");
                    return Enumerable.Empty<dynamic>();
                }

                // Unload
                if (parameters.Any(p => p.Key.Equals("Unload")))
                {
                    if (SeaShellHost.Env.Equals("_system"))
                    {
                        ConsoleIO.WriteWarning($"No active virtual environment to unload.");
                        return Enumerable.Empty<dynamic>();
                    }

                    LibraryManager.UnloadVirtual();
                    ConsoleIO.WriteInfo($"Unloaded virtual environment.");
                    return Enumerable.Empty<dynamic>();
                }

                // Create
                if (parameters.Any(p => p.Key.Equals("Create")))
                {
                    var dirName = Environment.CurrentDirectory;

                    parameters.TryGetValue("Create", out var CreateValue);
                    if (CreateValue != "")
                        dirName = CreateValue;

                    // TODO check for existing environment before overwriting
                    Directory.CreateDirectory(Path.Combine(dirName, "SeaShell.Environment"));
                    File.Create(Path.Combine(dirName, "SeaShell.Environment.ini")).Close();
                    VirtualEnv.WriteTemplateEnvironment(Path.Combine(dirName, "SeaShell.Environment.ini"));

                    ConsoleIO.WriteInfo($"Created virtual environment on {dirName}");
                    return Enumerable.Empty<dynamic>();
                }

                // Info
                if (parameters.Any(p => p.Key.Equals("Info")))
                {
                    if (SeaShellHost.Env.Equals("_system"))
                    {
                        ConsoleIO.WriteInfo($"No active virtual environment.");
                        return Enumerable.Empty<dynamic>();
                    }

                    ConsoleIO.WriteInfo($"Active virtual environment {SeaShellHost.Env}.");
                    return Enumerable.Empty<dynamic>();
                }
            }

            // More than one parameter present
            SeaShellErrors.NotifyMutuallyExclusive("Load", "Unload", "Create", "Info");
            return Enumerable.Empty<dynamic>();
        }
    }
}
