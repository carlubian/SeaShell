using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.IO
{
    public class CreateDirectoryCommand : ISeaShellCommand
    {
        public string Name => "Create-Directory";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Creates an empty directory.",
            Example = "Create-Directory [/Target] DirPath /Name DirName",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Specify the target directory. If missing, use current directory." },
                { "/Name", "Specify the name of the directory to create." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var dirName = Environment.CurrentDirectory;

            // Default parameter has a value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasNone("Target").Eval())
            {
                dirName = parameters.Single(p => p.Key == "_default").Value;
            }

            // Target parameter has a value
            if (Parameters.SeeIf(parameters).IsEmpty("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                dirName = parameters.Single(p => p.Key == "Target").Value;
            }

            // Both default and Target parameters have value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                return null;
            }

            // Name parameter missing or empty
            if (Parameters.SeeIf(parameters).HasNone("Name").Eval())
            {
                SeaShellErrors.NotifyMissingParam("Name");
                return null;
            }
            if (Parameters.SeeIf(parameters).IsEmpty("Name").Eval())
            {
                SeaShellErrors.NotifyParamMissingValue("Name");
                return null;
            }

            var NewPath = Path.Combine(dirName, parameters.Single(p => p.Key == "Name").Value);

            if (Directory.Exists(NewPath))
                ConsoleIO.WriteWarning($"Directory {NewPath} already exists.");
            else
                Directory.CreateDirectory(NewPath);

            return null;
        }
    }
}
