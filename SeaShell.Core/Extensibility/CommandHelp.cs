using System.Collections.Generic;

namespace SeaShell.Core.Extensibility
{
    public class CommandHelp
    {
        public string Description { get; set; } = "";
        public string Example { get; set; } = "";
        public IDictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
