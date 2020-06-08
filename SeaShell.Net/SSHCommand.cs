using Renci.SshNet;
using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SeaShell.Net
{
    public class SSHCommand : ISeaShellCommand
    {
        public string Name => "SSH";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Redirect to your platform SSH command.",
            Example = "SSH [/Target] host-ip [/User username]",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Target machine IP or host name." },
                { "/User", "User name to use in the connection." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var host = "";
            var user = "";

            // Target in /Target parameter
            if (Parameters.SeeIf(parameters).HasValue("Target").IsEmpty("_default").Eval())
            {
                host = parameters.Single(p => p.Key.Equals("Target")).Value;
            }
            // Target in default parameter
            else if (Parameters.SeeIf(parameters).HasValue("_default").HasNone("Target").Eval())
            {
                host = parameters.Single(p => p.Key.Equals("_default")).Value;
            }
            // Both default and Target parameter present
            else if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Target").Eval())
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
            // Neither default nor Target parameter present
            else
                SeaShellErrors.NotifyMissingOneOfParams("_default", "Target");

            // User parameter exists
            if (Parameters.SeeIf(parameters).HasParam("User").HasValue("User").Eval())
            {
                user = parameters.Single(p => p.Key.Equals("User")).Value;
            }

            // User specified in both Target and User parameters
            if (host.Contains("@") && user != "")
            {
                // Both users are the same
                if (host.Split("@")[0].Equals(user))
                    ConsoleIO.WriteWarning("Username should only be specified once.");
                // Users are different
                else
                {
                    ConsoleIO.WriteError($"Username conflict between {host.Split("@")[0]} and {user}.");
                    return null;
                }
            }

            DoExecute(host.Contains("@") ? host : user != "" ? $"{user}@{host}" : host);

            return null;
        }

        private void DoExecute(string connectionString)
        {
            Console.Write("SSH Password: > ");
            var password = Console.ReadLine();

            var client = new SshClient(connectionString.Split("@")[1], connectionString.Split("@")[0], password);
            client.Connect();
        }
    }
}
