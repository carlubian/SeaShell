using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public interface IParameterCheck
    {
        bool Eval(IEnumerable<Parameter> source);
    }
}
