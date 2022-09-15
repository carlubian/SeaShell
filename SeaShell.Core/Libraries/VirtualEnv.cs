using ConfigAdapter;
using System.IO;

namespace SeaShell.Core.Libraries
{
    internal class VirtualEnv
    {
#pragma warning disable CS8618 // El campo que acepta valores NULL está sin inicializar. Considere la posibilidad de declararlo como que acepta valores NULL.
        internal string Name { get; set; }
        internal string Description { get; set; }
#pragma warning restore CS8618 // El campo que acepta valores NULL está sin inicializar. Considere la posibilidad de declararlo como que acepta valores NULL.

        internal static VirtualEnv? Parse(string file)
        {
            if (!File.Exists(file))
                return null;

            var config = Configuration.From(file);

            return new VirtualEnv
            {
                Name = config.GetValue("Environment:Name") ?? string.Empty,
                Description = config.GetValue("Environment:Description") ?? string.Empty
            };
        }

        internal static void WriteTemplateEnvironment(string file)
        {
            using var writer = new StreamWriter(file);
            foreach (var line in new string[] {
                    "[Environment]",
                    "Name = VirtualEnv",
                    "Description = Description of your virtual environment here."
                })
                writer.WriteLine(line);
        }
    }
}
