using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using SeaShell.Core.Extensibility.DuckTyping;
using DotNet.Misc.Extensions.Linq;

namespace SeaShell.Reflection
{
    public class InjectStringCommand : ISeaShellCommand
    {
        public string Name => "Inject-String";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Injects a string into the pipeline, optionally preserving its contents.",
            Example = "Inject-String Foo [/Preserve]",
            Parameters = new Dictionary<string, string>
            {
                { "/Text (default)", "Text to add to the pipeline." },
                { "/Preserve", "Add string to the existing pipeline. If missing, replace it entirely." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (Or(And(ParamHasValue("_default"), ParamNotExists("Text")),
                And(ParamIsEmpty("_default"), ParamExists("Text"), ParamHasValue("Text"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var text))
                    parameters.TryGetValue("Text", out text);

                var pipeObj = new InjectStringPipelineObject
                {
                    StringValue = text
                };

                if (parameters.Any(p => p.Key.Equals("Preserve")))
                    return pipeline.Append(pipeObj);
                else
                    return pipeObj.Enumerate();
            }

            return Enumerable.Empty<dynamic>();
        }
    }

    public class InjectStringPipelineObject : IPipelinePrintable
    {
        public string StringValue { get; set; }
    }
}
