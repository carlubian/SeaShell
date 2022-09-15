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
            if (ParamHasValue("Name").Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var dirName))
                    if (!parameters.TryGetValue("Target", out dirName))
                        dirName = Environment.CurrentDirectory;

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

            return Enumerable.Empty<dynamic>();
        }
    }

    public class CreateDirectoryPipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
