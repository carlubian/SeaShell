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
            if(pipeline.Any())
            {
                foreach (var element in pipeline)
                    if (element is IPipelinePrintable ipp)
                        Console.WriteLine(ipp.StringValue);
            }
            else
            {
                if (!parameters.TryGetValue("_default", out var text))
                    if (!parameters.TryGetValue("Text", out text))
                    {
                        ConsoleIO.WriteError("Print command called outside a pipeline, or previous command returned empty object.");
                        return Enumerable.Empty<dynamic>();
                    }

                Console.WriteLine(text);
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
