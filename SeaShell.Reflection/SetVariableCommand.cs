using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System.Collections.Generic;
using System.Linq;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core;

namespace SeaShell.Reflection
{
    public class SetVariableCommand : ISeaShellCommand
    {
        public string Name => "Set-Variable";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Sets or overwrites a variable for the duration of this session.",
            Example = "Set-Variable /Key keyName /Value value",
            Parameters = new Dictionary<string, string>
            {
                { "/Key", "Name of the variable" },
                { "/Value", "Value of the variable" }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (ParamHasValue("Key").Eval(parameters))
            {
                parameters.TryGetValue("Key", out var key);

                if (pipeline.Any())
                {
                    if (pipeline.First() is IPipelineLocatable ipl)
                        SeaShellHost.Variables.Add(key, ipl.URI);
                    else if (pipeline.First() is IPipelinePrintable ipp)
                        SeaShellHost.Variables.Add(key, ipp.StringValue);
                }
                else if (ParamHasValue("Value").Eval(parameters))
                {
                    parameters.TryGetValue("Value", out var value);
                    SeaShellHost.Variables.Add(key, value);
                }
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
