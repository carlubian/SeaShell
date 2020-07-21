using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeaShell.Core;
using SeaShell.Core.Extensibility.DuckTyping;
using DotNet.Misc.Extensions.Linq;

namespace SeaShell.Reflection
{
    public class GetVariableCommand : ISeaShellCommand
    {
        public string Name => "Get-Variable";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Gets a variable from the current session.",
            Example = "Get-Variable [/Key] keyName",
            Parameters = new Dictionary<string, string>
            {
                { "/Key (default)", "Name of the variable." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (Or(ParamHasValue("_default"), ParamHasValue("Key")).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var key))
                    parameters.TryGetValue("Key", out key);

                var value = SeaShellHost.Variables[key];
                return new GetVariablePipelineObject(value).Enumerate();
            }

            return Enumerable.Empty<dynamic>();
        }
    }

    public class GetVariablePipelineObject : IPipelineLocatable, IPipelinePrintable
    {
        private string value;

        public GetVariablePipelineObject(string value)
        {
            this.value = value;
        }

        public string StringValue => value;

        public string URI => value;
    }
}
