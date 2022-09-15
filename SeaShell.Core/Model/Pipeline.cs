using System.Collections.Generic;

namespace SeaShell.Core.Model
{
    public class Pipeline
    {
        public IEnumerable<Command> Commands { get; set; }

        public Pipeline(IEnumerable<Command> commands)
        {
            Commands = commands;
        }

        public override string ToString() => string.Join(" > ", Commands);
    }
}
