using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParameterCheckBuilder
    {
        internal IList<IParameterCheck> checks = new List<IParameterCheck>();
        internal IEnumerable<Parameter> source;

        public ParameterCheckBuilder(IEnumerable<Parameter> source)
        {
            this.source = source;
        }

        public ParameterCheckBuilder MustBePresent(string name)
        {
            checks.Add(new MustBePresent(name));
            return this;
        }

        public ParameterCheckBuilder MustHaveValue(string name)
        {
            checks.Add(new MustHaveValue(name));
            return this;
        }

        public ParameterCheckBuilder OnlyOneCanBePresent(params string[] values)
        {
            checks.Add(new OnlyOneCanBePresent(values));
            return this;
        }

        public bool Execute()
        {
            foreach (var check in checks)
            {
                if (check.Invoke(source) is false)
                    return false;
            }
            return true;
        }
    }
}
