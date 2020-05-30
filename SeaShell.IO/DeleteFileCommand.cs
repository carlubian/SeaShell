using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Extensibility.Parameters;
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
            {
                // Default parameter has a value
                if (Parameters.SeeIf(parameters).HasValue("_default").HasNone("Target").Eval())
                {
                    dirName = parameters.Single(p => p.Key == "_default").Value;
                    DoDeleteFile(dirName);
                    return null;
                }

                // Target parameter has a value
                if (Parameters.SeeIf(parameters).IsEmpty("_default").HasParam("Target")
                    .HasValue("Target").Eval())
                {
                    dirName = parameters.Single(p => p.Key == "Target").Value;
                    DoDeleteFile(dirName);
                    return null;
                }

                // Both default and Target parameters have value
                if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Target")
                    .HasValue("Target").Eval())
                {
                    SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                    return null;
                }
            }

            // Case 2: File(s) come from pipeline
            {
                foreach (var item in pipeline)
                    if (item is IPipelineLocatable loc)
                        DoDeleteFile(loc.URI);
            }

            return null;
        }

        private void DoDeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
