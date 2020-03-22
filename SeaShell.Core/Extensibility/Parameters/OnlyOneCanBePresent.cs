using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class OnlyOneCanBePresent : IParameterCheck
    {
        private readonly IEnumerable<string> Values;

        public OnlyOneCanBePresent(IEnumerable<string> values)
        {
            Values = values;
        }

        public bool Invoke(IEnumerable<Parameter> source)
        {
            var intersection = source.Select(p => p.Key.Content)
                .Intersect(Values);

            return intersection.Count() <= 1;
        }
    }
}
