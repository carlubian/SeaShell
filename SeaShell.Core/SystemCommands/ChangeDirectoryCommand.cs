using DotNet.Misc.Extensions.Linq;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.SystemCommands
{
    public class ChangeDirectoryCommand : ISeaShellCommand
    {
        public string Name => "Change-Directory";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Changes the active directory.",
            Example = "Change-Directory [/Target] Path",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Target directory path." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var newPath = Environment.CurrentDirectory;

            // Default parameter has a value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasNone("Target").Eval())
            {
                newPath = parameters.Single(p => p.Key == "_default").Value;
            }

            // Target parameter has a value
            if (Parameters.SeeIf(parameters).IsEmpty("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                newPath = parameters.Single(p => p.Key == "Target").Value;
            }

            // Both default and Target parameters have value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                return null;
            }

            Environment.CurrentDirectory = newPath;

            return new ChangeDirectoryPipelineObject
            {
                URI = Environment.CurrentDirectory
            }.Enumerate();
        }
    }

    public class ChangeDirectoryPipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
