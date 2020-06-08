using ConfigAdapter.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Libraries
{
    internal class VirtualEnv
    {
        internal string Name { get; set; }
        internal string Description { get; set; }

        internal static VirtualEnv Parse(string file)
        {
            if (!File.Exists(file))
                return null;

            var config = IniConfig.From(file);

            return new VirtualEnv
            {
                Name = config.Read("Environment:Name"),
                Description = config.Read("Environment:Description")
            };
        }

        internal static void WriteTemplateEnvironment(string file)
        {
            using (var writer = new StreamWriter(file))
                foreach (var line in new string[] {
                    "[Environment]",
                    "Name = VirtualEnv",
                    "Description = Description of your virtual environment here."
                })
                    writer.WriteLine(line);
        }
    }
}
