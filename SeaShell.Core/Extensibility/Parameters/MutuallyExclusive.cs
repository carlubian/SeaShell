using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class MutuallyExclusive : IParameterCheck
    {
        private readonly IEnumerable<string> Values;

        public MutuallyExclusive(IEnumerable<string> values)
        {
            Values = values;
        }

        public bool Eval(IEnumerable<Parameter> source)
        {
            var intersection = source.Select(p => p.Key.Content)
                .Intersect(Values);

            var result = intersection.Count() <= 1;

            if (!result)
                SeaShellErrors.NotifyMutuallyExclusive(Values.ToArray());

            return result;
        }
    }
}
