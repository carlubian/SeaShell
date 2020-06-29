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
    public class CreateDirectoryCommand : ISeaShellCommand
    {
        public string Name => "Create-Directory";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Creates an empty directory.",
            Example = "Create-Directory [/Target] DirPath /Name DirName",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Specify the target directory. If missing, use current directory." },
                { "/Name", "Specify the name of the directory to create." }
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
                return Enumerable.Empty<dynamic>();
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

            var NewPath = Path.Combine(dirName, parameters.Single(p => p.Key == "Name").Value);

            if (Directory.Exists(NewPath))
                ConsoleIO.WriteWarning($"Directory {NewPath} already exists.");
            else
                Directory.CreateDirectory(NewPath);

            return new CreateDirectoryPipelineObject
            {
                URI = NewPath
            }.Enumerate();
        }
    }

    public class CreateDirectoryPipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
