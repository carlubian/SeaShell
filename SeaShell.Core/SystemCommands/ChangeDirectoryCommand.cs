using DotNet.Misc.Extensions.Linq;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;

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
            if (pipeline.Any())
            {
                foreach (var element in pipeline)
                    if (element is IPipelineLocatable ipl)
                    {
                        Environment.CurrentDirectory = ipl.URI;
                        break;
                    }
            }
            else
            {
                if (And(Or(ParamHasValue("_default"), ParamHasValue("Target")),
                MutuallyExclusive("_default", "Target")).Eval(parameters))
                {
                    if (!parameters.TryGetValue("_default", out var newPath))
                        parameters.TryGetValue("Target", out newPath);

                    Environment.CurrentDirectory = newPath;
                }
            }

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
