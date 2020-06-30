﻿using SeaShell.Core.Extensibility;
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
    public class DeleteFileCommand : ISeaShellCommand
    {
        public string Name => "Delete-File";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Deletes a file.",
            Example = "Delete-File [/Target] File.txt",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Path to the file." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var dirName = Environment.CurrentDirectory;

            // Case 1: File in Target param or _default param
            if (Or(And(ParamHasValue("_default"), ParamNotExists("Target")),
                And(ParamIsEmpty("_default"), ParamExists("Target"), ParamHasValue("Target"))).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out dirName))
                    parameters.TryGetValue("Target", out dirName);

                DoDeleteFile(dirName);
            }
            else
            // Case 2: File(s) come from pipeline
            {
                foreach (var item in pipeline)
                    if (item is IPipelineLocatable loc)
                        DoDeleteFile(loc.URI);
            }

            return Enumerable.Empty<dynamic>();
        }

        private void DoDeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
