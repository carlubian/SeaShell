using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class MustBePresent : IParameterCheck
    {
        private readonly string Name;

        public MustBePresent(string name)
        {
            Name = name;
        }

        public bool Invoke(IEnumerable<Parameter> source) => source.Any(p => p.Key.Equals(Name));
    }
}
