using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParamHasValue : IParameterCheck
    {
        private readonly string Name;

        public ParamHasValue(string name)
        {
            Name = name;
        }

        public bool Eval(IEnumerable<Parameter> source)
        {
            var result = source.FirstOrDefault(p => p.Key.Equals(Name))?.Value != "";

            if (!result)
                SeaShellErrors.NotifyParamMissingValue(Name);

            return result;
        }
    }
}
