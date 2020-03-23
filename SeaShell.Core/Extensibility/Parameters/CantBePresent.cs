using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class CantBePresent : IParameterCheck
    {
        private readonly string Name;

        public CantBePresent(string name)
        {
            Name = name;
        }

        public bool Invoke(IEnumerable<Parameter> source) => !source.Any(p => p.Key.Equals(Name));
    }
}
