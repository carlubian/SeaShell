using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using System.Collections.Generic;
using System.Net;
using SeaShell.Core.Extensibility.DuckTyping;
using System.IO;
using DotNet.Misc.Extensions.Linq;
using System.Linq;

namespace SeaShell.Net
{
    public class FetchUriCommand : ISeaShellCommand
    {
        public string Name => "Fetch-Uri";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Obtains a file from a specified URI.",
            Example = "Fetch-Uri \"http://www.contoso.com/foo.txt\" /Local foo.txt",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "The URI pointing to the file resource" },
                { "/Local", "Name that will be given to the obtained file" }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (And(Or(ParamHasValue("_default"), And(ParamExists("Target"), ParamHasValue("Target"))),
                ParamExists("Local"), ParamHasValue("Local")).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var uri))
                    parameters.TryGetValue("Target", out uri);
                parameters.TryGetValue("Local", out var local);

                using var client = new WebClient();
                client.DownloadFile(uri, local);

                return new FetchUriPipelineObject
                {
                    URI = new FileInfo(local).FullName
                }.Enumerate();
            }

            return Enumerable.Empty<dynamic>();
        }
    }

    public class FetchUriPipelineObject : IPipelineLocatable
    {
        public string URI { get; set; }
    }
}
