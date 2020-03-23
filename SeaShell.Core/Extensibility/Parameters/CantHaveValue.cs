using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class CantHaveValue : IParameterCheck
    {
        private string Name;

        public CantHaveValue(string name)
        {
            Name = name;
        }

        public bool Invoke(IEnumerable<Parameter> source) => source.Single(p => p.Key.Equals(Name)).Value == "";
    }
}
