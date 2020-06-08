using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class MustHaveValue : IParameterCheck
    {
        private string Name;

        public MustHaveValue(string name)
        {
            Name = name;
        }

        public bool Invoke(IEnumerable<Parameter> source) => source.FirstOrDefault(p => p.Key.Equals(Name))?.Value != "";
    }
}
