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
using System.Text;

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

            // Default parameter has a value
            if (And(ParamHasValue("_default"), ParamNotExists("Target")).Eval(parameters))
            {
                parameters.TryGetValue("_default", out dirName);
            }

            // Target parameter has a value
            if (And(ParamIsEmpty("_default"), ParamExists("Target"), ParamHasValue("Target")).Eval(parameters))
            {
                parameters.TryGetValue("Target", out dirName);
            }

            // Both default and Target parameters have value
            if (And(ParamHasValue("_default"), ParamExists("Target"), ParamHasValue("Target")).Eval(parameters))
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                return null;
            }

            // Name parameter missing or empty
            if (ParamNotExists("Name").Eval(parameters))
            {
                SeaShellErrors.NotifyMissingParam("Name");
                return Enumerable.Empty<dynamic>();
            }
            else if (ParamIsEmpty("Name").Eval(parameters))
            {
                SeaShellErrors.NotifyParamMissingValue("Name");
                return null;
            }

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
    }

    public class CreateFilePipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
