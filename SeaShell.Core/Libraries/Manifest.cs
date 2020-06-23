using ConfigAdapter.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Libraries
{
    internal class Manifest
    {
#pragma warning disable CS8618 // El campo que acepta valores NULL está sin inicializar. Considere la posibilidad de declararlo como que acepta valores NULL.
        internal string ManifestVersion { get; set; }
        internal string HostVersion { get; set; }
        internal string RuntimeVersion { get; set; }

        internal string Name { get; set; }
        internal string LibraryVersion { get; set; }
        internal string Author { get; set; }
        internal string URI { get; set; }
        internal string Description { get; set; }
        internal string[] Assemblies { get; set; }
#pragma warning restore CS8618 // El campo que acepta valores NULL está sin inicializar. Considere la posibilidad de declararlo como que acepta valores NULL.

        internal static Manifest? Parse(string file)
        {
            if (!File.Exists(file))
                return null;

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

        internal static void WriteTemplateManifest(string file)
        {
            using (var writer = new StreamWriter(file))
                foreach (var line in new string[] {
                    "[SeaShell]",
                    "Manifest Version = 1",
                    "Host Version = 0.2 - 1.0",
                    "Runtime Version = 3.1",
                    "",
                    "[Library]",
                    "Name = Replace.With.Library.Name",
                    "Version = 1.0.0",
                    "Author = Your name or email here",
                    "URI = https://www.github.com/YourUserHere/YourRepositoryHere",
                    "Description = Description of your library here.",
                    "Assemblies = YourAssembly.dll"
                })
                    writer.WriteLine(line);
        }
    }
}
