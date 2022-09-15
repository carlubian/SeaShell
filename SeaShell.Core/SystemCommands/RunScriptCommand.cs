using SeaShell.Core.Extensibility;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;

namespace SeaShell.Core.SystemCommands
{
    public class RunScriptCommand : ISeaShellCommand
    {
        public string Name => "Run-Script";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Run a SeaShell Script file.",
            Example = "Run-Script [/Target] Path",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Path to the script file. If missing, get the value from the pipeline." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            // Case 1: Path in _default or Target param
            // Case 2: Path comes from pipeline

            throw new NotImplementedException();
        }
    }
}
