using DotNet.Misc.Extensions.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Model
{
    public class Pipeline
    {
        public IEnumerable<Command> Commands { get; set; }

        public Pipeline(IEnumerable<Command> commands)
        {
            Commands = commands;
        }

        public override string ToString() => Commands.Stringify(c => c.ToString(), " > ");
    }
}
