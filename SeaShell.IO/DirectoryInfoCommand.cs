using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.IO
{
    public class DirectoryInfoCommand : ISeaShellCommand
    {
        public string Name => "Directory-Info";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Displays information about a directory.",
            Example = "Directory-Info [/Target] Path",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Path to the directory. If missing, get the value from the pipeline or use current directory." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var path = "";

            // Default parameter and Target parameter present
            if (And(ParamHasValue("_default"), ParamExists("Target"), ParamHasValue("Target")).Eval(parameters))
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                return null;
            }

            // Target parameter without value
            if (And(ParamExists("Target"), ParamIsEmpty("Target")).Eval(parameters))
            {
                SeaShellErrors.NotifyParamMissingValue("Target");
                return null;
            }

            // Default parameter with value
            if (And(ParamHasValue("_default"), ParamNotExists("Target")).Eval(parameters))
            {
                parameters.TryGetValue("_default", out path);
            }

            // Target parameter with value
            if (And(ParamIsEmpty("_default"), ParamExists("Target"), ParamHasValue("Target")).Eval(parameters))
            {
                parameters.TryGetValue("Target", out path);
            }

            if (path != "")
            {
                if (!File.Exists(path))
                    ConsoleIO.WriteError($"Directory {path} doesn't exist.");
                else
                    DoShowInfo(path);
            }
            else
            {
                // Get pipeline object
                if (pipeline is null)
                {
                    DoShowInfo(Environment.CurrentDirectory);
                    return null;
                }

                foreach (var obj in pipeline)
                    if (obj is IPipelineLocatable ipp)
                        DoShowInfo(ipp.URI);
            }

            return null;
        }

        private void DoShowInfo(string path)
        {
            var dir = new DirectoryInfo(path);
            Console.Write("Directory name: ");
            ConsoleIO.WriteInfo(dir.Name);
            Console.Write("Last modified: ");
            ConsoleIO.WriteInfo(dir.LastWriteTime.ToString("dd-MMM-yyyy hh:mm"));
            Console.Write("Attributes: ");
            ConsoleIO.WriteInfo(dir.Attributes.ToString());
            Console.WriteLine("Use Enumerate-Directory to see files and subdirectories.");
        }
    }
}
