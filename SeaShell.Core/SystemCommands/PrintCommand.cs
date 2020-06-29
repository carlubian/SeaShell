using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.SystemCommands
{
    public class PrintCommand : ISeaShellCommand
    {
        public string Name => "Print";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Prints a message to the console.",
            Example = "Print [/Text] Text Content",
            Parameters = new Dictionary<string, string>
            {
                { "/Text (default)", "Text to print. If missing, get the value from the pipeline." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var text = "";

            // Default parameter and Text parameter present
            if (And(ParamHasValue("_default"), ParamExists("Text"), ParamHasValue("Text")).Eval(parameters))
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Text");
                return Enumerable.Empty<dynamic>();
            }

            // Text parameter without value
            if (And(ParamExists("Text"), ParamIsEmpty("Text")).Eval(parameters))
            {
                SeaShellErrors.NotifyParamMissingValue("Text");
                return Enumerable.Empty<dynamic>();
            }

            // Default parameter with value
            if (And(ParamHasValue("_default"), ParamNotExists("Text")).Eval(parameters))
            {
                parameters.TryGetValue("_default", out text);
            }

            // Text parameter with value
            if (And(ParamIsEmpty("_default"), ParamExists("Text"), ParamHasValue("Text")).Eval(parameters))
            {
                parameters.TryGetValue("Text", out text);
            }

            if (text != "")
            {
                Console.WriteLine(text);
            }
            else
            {
                // Get pipeline object
                if (pipeline is null)
                {
                    ConsoleIO.WriteError("Print command called outside a pipeline, or previous command returned empty object.");
                    return Enumerable.Empty<dynamic>();
                }

                foreach (var obj in pipeline)
                {
                    if (obj is IPipelinePrintable ipp)
                        Console.WriteLine(ipp.StringValue);
                }
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
