using SeaShell.Core.Extensibility;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System.Collections.Generic;
using System.Linq;
using SeaShell.Core;

namespace SeaShell.Reflection
{
    public class EvalStringCommand : ISeaShellCommand
    {
        public string Name => "Eval-String";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Interprets a string as a SeaShell command, and executes it.",
            Example = "Run-String \"Enumerate-Directory .\"",
            Parameters = new Dictionary<string, string>
            {
                { "/Text (default)", "Text to evaluate as a command." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (Or(And(ParamHasValue("_default"), ParamNotExists("Text")),
                And(ParamIsEmpty("_default"), ParamExists("Text"), ParamHasValue("Text"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var text))
                    parameters.TryGetValue("Text", out text);

                SeaShellHost.ParseAndRun(text);
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
