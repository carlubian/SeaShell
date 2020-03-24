using ConfigAdapter.Ini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Libraries
{
    internal class Manifest
    {
        internal string ManifestVersion { get; set; }
        internal string HostVersion { get; set; }
        internal string RuntimeVersion { get; set; }

        internal string Name { get; set; }
        internal string LibraryVersion { get; set; }
        internal string Author { get; set; }
        internal string URI { get; set; }
        internal string Description { get; set; }
        internal string[] Assemblies { get; set; }

        internal static Manifest Parse(string file)
        {
            var config = IniConfig.From(file);

            return new Manifest
            {
                ManifestVersion = config.Read("SeaShell:Manifest Version"),
                HostVersion = config.Read("SeaShell:Host Version"),
                RuntimeVersion = config.Read("SeaShell:Runtime Version"),
                Name = config.Read("Library:Name"),
                LibraryVersion = config.Read("Library:Version"),
                Author = config.Read("Library:Author"),
                URI = config.Read("Library:URI"),
                Description = config.Read("Library:Description"),
                Assemblies = config.Read("Library:Assemblies").Split(',').Select(a => a.Trim()).ToArray()
            };
        }
    }
}
