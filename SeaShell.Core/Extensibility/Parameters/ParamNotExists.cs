using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParamNotExists : IParameterCheck
    {
        private readonly string Name;

        public ParamNotExists(string name)
        {
            Name = name;
        }

        public bool Eval(IEnumerable<Parameter> source)
        {
            var result = source.Any(p => p.Key.Equals(Name));

            if (result)
                SeaShellErrors.NotifyParamPresent(Name);

            return !result;
        }
    }
}
