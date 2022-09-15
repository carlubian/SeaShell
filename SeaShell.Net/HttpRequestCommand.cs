using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using static SeaShell.Core.Extensibility.Parameters.ParameterCheckBuilder;
using System.Collections.Generic;
using SeaShell.Core.Extensibility.DuckTyping;
using DotNet.Misc.Extensions.Linq;
using System.Linq;
using System.Net.Http;

namespace SeaShell.Net
{
    public class HttpRequestCommand : ISeaShellCommand
    {
        public string Name => "Http-Request";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Sends a request to a specified URI.",
            Example = "Http-Request \"http://www.contoso.com/foo.txt\" /Method GET",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "The URI to send the request to" },
                { "/Method", "GET, POST, PUT, DELETE" }
                // TODO implement body form parameters for POST and PUT
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            if (And(Or(ParamHasValue("_default"), And(ParamExists("Target"), ParamHasValue("Target"))),
                ParamExists("Method"), ParamHasValue("Method")).Eval(parameters))
            {
                if (!parameters.TryGetValue("_default", out var uri))
                    parameters.TryGetValue("Target", out uri);
                parameters.TryGetValue("Method", out var method);

                HttpResponseMessage response;
                string result;
                using var client = new HttpClient();
                
                switch (method)
                {
                    case "GET":
                        var task = client.GetAsync(uri);
                        task.Wait();
                        response = task.Result;
                        var task2 = response.Content.ReadAsStringAsync();
                        task2.Wait();
                        result = task2.Result;
                        break;
                    case "POST":
                        task = client.PostAsync(uri, new StringContent(""));
                        task.Wait();
                        response = task.Result;
                        task2 = response.Content.ReadAsStringAsync();
                        task2.Wait();
                        result = task2.Result;
                        break;
                    case "PUT":
                        task = client.PutAsync(uri, new StringContent(""));
                        task.Wait();
                        response = task.Result;
                        task2 = response.Content.ReadAsStringAsync();
                        task2.Wait();
                        result = task2.Result;
                        break;
                    case "DELETE":
                        task = client.DeleteAsync(uri);
                        task.Wait();
                        response = task.Result;
                        task2 = response.Content.ReadAsStringAsync();
                        task2.Wait();
                        result = task2.Result;
                        break;
                    default:
                        result = "";
                        break;
                }

                return new HttpRequestPipelineObject
                {
                    StringValue = result
                }.Enumerate();
            }

            return Enumerable.Empty<dynamic>();
        }
    }

    public class HttpRequestPipelineObject : IPipelinePrintable
    {
        public string StringValue { get; set; }
    }
}
