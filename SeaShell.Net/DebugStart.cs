using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Net
{
    public static class DebugStart
    {
        public static void Main(string[] args)
        {
            var ssh = new SSHCommand();
            var parameters = new List<Parameter>
            {
                new Parameter("_default", "192.168.0.65"),
                new Parameter("User", "pi")
            };

            ssh.Invoke(parameters, null);
        }
    }
}
