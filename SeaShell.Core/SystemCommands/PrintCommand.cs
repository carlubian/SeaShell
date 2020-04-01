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
            if (Parameters.SeeIf(parameters).HasParam("_default").HasParam("Text").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Text");
                return null;
            }

            // Text parameter without value
            if (Parameters.SeeIf(parameters).HasParam("Text").IsEmpty("Text").Eval())
            {
                SeaShellErrors.NotifyParamMissingValue("Text");
                return null;
            }

            // Default parameter with value
            if (Parameters.SeeIf(parameters).HasValue("_default").Eval())
            {
                text = parameters.Single(p => p.Key == "_default").Value;
            }

            // Text parameter with value
            if (Parameters.SeeIf(parameters).HasParam("Text").HasValue("Text").Eval())
            {
                text = parameters.Single(p => p.Key == "Text").Value;
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
                    return null;
                }

                foreach (var obj in pipeline)
                {
                    if (obj is IPipelinePrintable ipp)
                        Console.WriteLine(ipp.StringValue);
                }
            }

            return null;
        }
    }
}
