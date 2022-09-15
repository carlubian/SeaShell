using DotNet.Misc.Extensions.Linq;
using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeaShell.IO
{
    public class CreateFileCommand : ISeaShellCommand
    {
        public string Name => "Create-File";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Creates an empty file.",
            Example = "Create-File [/Target] Path /Name File.txt",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Path to the container directory. If missing, use current directory." },
                { "/Name", "Name and extension of the new file." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var dirName = Environment.CurrentDirectory;

            if (Or(And(ParamHasValue("_default"), ParamNotExists("Target"), ParamExists("Name"), ParamHasValue("Name")),
                And(ParamIsEmpty("_default"), ParamExists("Target"), ParamHasValue("Target"), ParamExists("Name"), ParamHasValue("Name"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out dirName))
                    parameters.TryGetValue("Target", out dirName);

                var fileName = parameters.Single(p => p.Key == "Name").Value;

                if (!Directory.Exists(dirName))
                    Directory.CreateDirectory(dirName);

                if (File.Exists(Path.Combine(dirName, fileName)))
                    ConsoleIO.WriteWarning($"File {Path.Combine(dirName, fileName)} already exists.");
                else
                    File.Create(Path.Combine(dirName, fileName));

                return new CreateFilePipelineObject
                {
                    URI = Path.Combine(dirName, fileName)
                }.Enumerate();
            }

            return Enumerable.Empty<dynamic>();
        }
    }

    public class CreateFilePipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
