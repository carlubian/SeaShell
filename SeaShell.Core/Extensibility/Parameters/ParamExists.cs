using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParamExists : IParameterCheck
    {
        private readonly string Name;

        public ParamExists(string name)
        {
            Name = name;
        }

        public bool Eval(IEnumerable<Parameter> source, bool silent = false)
        {
            var result = source.Any(p => p.Key.Equals(Name));

            if (!result && !silent)
                SeaShellErrors.NotifyMissingParam(Name);

            return result;
        }
    }
}
