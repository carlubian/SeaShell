using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public static class Parameters
    {
        public static ParameterCheckBuilder SeeIf(IEnumerable<Parameter> source) => new ParameterCheckBuilder(source);
    }
}
