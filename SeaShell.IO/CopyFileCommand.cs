using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Extensibility.Parameters;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using SeaShell.Core.Model;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SeaShell.IO
{
    public class CopyFileCommand : ISeaShellCommand
    {
        public string Name => "Copy-File";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Copies a file.",
            Example = "Copy-File /Source path\\to\\file.txt /Target path\\to\\destination",
            Parameters = new Dictionary<string, string>
            {
                { "/Source", "File to be copied." },
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
                            DoCopyFile(ipl.URI, target);

                    return pipeline;
                }
                else
                {
                    if (parameters.TryGetValue("Source", out var source))
                        DoCopyFile(source, target);
                }
            }

            return pipeline;
        }

        private void DoCopyFile(string source, string target)
        {
            if (!File.Exists(source))
                return;

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);

            var fileName = new FileInfo(source).Name;

            File.Copy(source, Path.Combine(target, fileName), true);
        }
    }
}
