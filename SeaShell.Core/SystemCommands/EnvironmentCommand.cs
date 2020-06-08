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
            // More than one parameter present
            if (!Parameters.SeeIf(parameters).HasOnlyOne("Load", "Unload", "Create", "Info").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("Load", "Unload", "Create", "Info");
                return null;
            }

            // Load
            if (Parameters.SeeIf(parameters).HasParam("Load").Eval())
            {
                var dirName = Environment.CurrentDirectory;

                if (parameters.Single(p => p.Key == "Load").Value != "")
                    dirName = parameters.Single(p => p.Key == "Load").Value;

                // TODO check for errors (missing directory, incorrect format)
                LibraryManager.LoadVirtual(dirName);
                ConsoleIO.WriteInfo($"Loaded virtual environment {SeaShellHost.Env}");
                return null;
            }

            // Unload
            if (Parameters.SeeIf(parameters).HasParam("Unload").Eval())
            {
                if (SeaShellHost.Env.Equals("_system"))
                {
                    ConsoleIO.WriteWarning($"No active virtual environment to unload.");
                    return null;
                }

                LibraryManager.UnloadVirtual();
                ConsoleIO.WriteInfo($"Unloaded virtual environment.");
                return null;
            }

            // Create
            if (Parameters.SeeIf(parameters).HasParam("Create").Eval())
            {
                var dirName = Environment.CurrentDirectory;

                if (parameters.Single(p => p.Key == "Create").Value != "")
                    dirName = parameters.Single(p => p.Key == "Create").Value;

                // TODO check for existing environment before overwriting
                Directory.CreateDirectory(Path.Combine(dirName, "SeaShell.Environment"));
                File.Create(Path.Combine(dirName, "SeaShell.Environment.ini")).Close();
                VirtualEnv.WriteTemplateEnvironment(Path.Combine(dirName, "SeaShell.Environment.ini"));

                ConsoleIO.WriteInfo($"Created virtual environment on {dirName}");
                return null;
            }

            // Info
            if (Parameters.SeeIf(parameters).HasParam("Info").Eval())
            {
                if (SeaShellHost.Env.Equals("_system"))
                {
                    ConsoleIO.WriteInfo($"No active virtual environment.");
                    return null;
                }

                ConsoleIO.WriteInfo($"Active virtual environment {SeaShellHost.Env}.");
                return null;
            }

            // No parameter present
            SeaShellErrors.NotifyMissingOneOfParams("Load", "Unload", "Create", "Info");
            return null;
        }
    }
}
