using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class ParameterCheckBuilder : IParameterCheckBuilder
    {
        public IParameterCheckBuilder And(params IParameterCheck[] @params) => new AndParamCheckBuilder(@params);
        public IParameterCheckBuilder Or(params IParameterCheck[] @params) => throw new NotImplementedException();
        public IParameterCheck ParamExists(string param) => new ParamExists(param);
        public IParameterCheck ParamNotExists(string param) => new ParamNotExists(param);
        public IParameterCheck ParamHasValue(string param) => new ParamHasValue(param);
        public IParameterCheck ParamIsEmpty(string param) => new ParamIsEmpty(param);
        public IParameterCheck MutuallyExclusive(params string[] @params) => new MutuallyExclusive(@params);
    }
}
