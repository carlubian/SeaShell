using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParamIsEmpty : IParameterCheck
    {
        private readonly string Name;

        public ParamIsEmpty(string name)
        {
            Name = name;
        }

        public bool Eval(IEnumerable<Parameter> source)
        {
            var result = source.FirstOrDefault(p => p.Key.Equals(Name))?.Value != "";

            if (result)
                SeaShellErrors.NotifyParamValuePresent(Name);

            return !result;
        }
    }
}
