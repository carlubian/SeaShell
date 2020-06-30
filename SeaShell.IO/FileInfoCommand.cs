using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.IO
{
    public class FileInfoCommand : ISeaShellCommand
    {
        public string Name => "File-Info";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Displays information about a file.",
            Example = "File-Info [/Target] File.txt",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Path to the file. If missing, get the value from the pipeline." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (Or(And(ParamHasValue("_default"), ParamNotExists("Target")),
                And(ParamIsEmpty("_default"), ParamExists("Target"), ParamHasValue("Target"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var path))
                    parameters.TryGetValue("Target", out path);

                if (path != "")
                {
                    if (!File.Exists(path))
                        ConsoleIO.WriteError($"File {path} doesn't exist.");
                    else
                    {
                        var file = new FileInfo(path);
                        Console.Write("File name: ");
                        ConsoleIO.WriteInfo(file.Name);
                        Console.Write("Extension: ");
                        ConsoleIO.WriteInfo(file.Extension);
                        Console.Write("Length: ");
                        ConsoleIO.WriteInfo(file.Length.ToString());
                        Console.Write("Attributes: ");
                        ConsoleIO.WriteInfo(file.Attributes.ToString());
                    }
                }
            }
            else
            {
                // Get pipeline object
                if (pipeline is null)
                {
                    ConsoleIO.WriteError("File-Info command called outside a pipeline, or previous command returned empty object.");
                    return null;
                }

                foreach (var obj in pipeline)
                {
                    if (obj is IPipelineLocatable ipp)
                    {
                        var file = new FileInfo(ipp.URI);
                        Console.Write("File name: ");
                        ConsoleIO.WriteInfo(file.Name);
                        Console.Write("Extension: ");
                        ConsoleIO.WriteInfo(file.Extension);
                        Console.Write("Length: ");
                        ConsoleIO.WriteInfo(file.Length.ToString());
                        Console.Write("Attributes: ");
                        ConsoleIO.WriteInfo(file.Attributes.ToString());
                    }
                }
            }

            return Enumerable.Empty<dynamic>();
        }
    }
}
