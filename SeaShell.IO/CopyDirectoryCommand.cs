using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Extensibility.Parameters;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SeaShell.IO
{
    public class CopyDirectoryCommand : ISeaShellCommand
    {
        public string Name => "Copy-Directory";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Copies a directory.",
            Example = "Copy-Directory /Source path\\to\\origin /Target path\\to\\destination",
            Parameters = new Dictionary<string, string>
            {
                { "/Source", "Directory to be copied." },
                { "/Target", "Directory to place the copy in." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (ParamHasValue("Target").Eval(parameters))
            {
                parameters.TryGetValue("Target", out var target);

                if (pipeline.Any())
                {
                    foreach (var element in pipeline)
                        if (element is IPipelineLocatable ipl)
                            DoCopyDirectory(ipl.URI, target);

                    return pipeline;
                }
                else
                {
                    if (parameters.TryGetValue("Source", out var source))
                        DoCopyDirectory(source, target);
                }
            }

            return pipeline;
        }

        private void DoCopyDirectory(string source, string target)
        {
            if (!Directory.Exists(source))
                return;

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            foreach (var file in Directory.EnumerateFiles(source))
            {
                var fileName = new FileInfo(file).Name;
                File.Copy(file, Path.Combine(target, fileName), true);
            }

            foreach (var directory in Directory.EnumerateDirectories(source))
            {
                var dirName = new DirectoryInfo(directory).Name;
                DoCopyDirectory(directory, Path.Combine(target, dirName));
            }
        }
    }
}
