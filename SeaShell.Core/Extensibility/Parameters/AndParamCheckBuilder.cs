using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public class AndParamCheckBuilder : IParameterCheckBuilder, IParameterCheck
    {
        private readonly IParameterCheck[] @params;

        public AndParamCheckBuilder(params IParameterCheck[] @params)
        {
            this.@params = @params;
        }

        public IParameterCheckBuilder And(params IParameterCheck[] param) => new AndParamCheckBuilder(@params.Union(param).ToArray());
        public IParameterCheckBuilder Or(params IParameterCheck[] param) => new AndParamCheckBuilder(@params.Append(new OrParamCheckBuilder(param)).ToArray());
        public IParameterCheck ParamExists(string param) => new AndParamCheckBuilder(@params.Append(new ParamExists(param)).ToArray());
        public IParameterCheck ParamNotExists(string param) => new AndParamCheckBuilder(@params.Append(new ParamNotExists(param)).ToArray());
        public IParameterCheck ParamHasValue(string param) => new AndParamCheckBuilder(@params.Append(new ParamHasValue(param)).ToArray());
        public IParameterCheck ParamIsEmpty(string param) => new AndParamCheckBuilder(@params.Append(new ParamIsEmpty(param)).ToArray());
        public IParameterCheck MutuallyExclusive(params string[] @param) => new AndParamCheckBuilder(@params.Append(new MutuallyExclusive(@param)).ToArray());

        public bool Eval(IEnumerable<Parameter> source) => @params.All(p => p.Eval(source));
    }
}
