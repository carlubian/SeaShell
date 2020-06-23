using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public interface IParameterCheckBuilder
    {
        IParameterCheck ParamExists(string param);

        IParameterCheck ParamNotExists(string param);

        IParameterCheck ParamHasValue(string param);

        IParameterCheck ParamIsEmpty(string param);

        IParameterCheck MutuallyExclusive(params string[] @params);

        IParameterCheckBuilder And(params IParameterCheck[] @params);

        IParameterCheckBuilder Or(params IParameterCheck[] @params);
    }
}
