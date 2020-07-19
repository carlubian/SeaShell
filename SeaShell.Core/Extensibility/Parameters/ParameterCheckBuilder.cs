using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Extensibility.Parameters
{
    public static class ParameterCheckBuilder
    {
        public static AndParamCheckBuilder And(params IParameterCheck[] @params) => new AndParamCheckBuilder(@params);
        public static OrParamCheckBuilder Or(params IParameterCheck[] @params) => new OrParamCheckBuilder(@params);
        public static IParameterCheck ParamExists(string param) => new ParamExists(param);
        public static IParameterCheck ParamNotExists(string param) => new ParamNotExists(param);
        public static IParameterCheck ParamHasValue(string param) => new ParamHasValue(param);
        public static IParameterCheck ParamIsEmpty(string param) => new ParamIsEmpty(param);
        public static IParameterCheck MutuallyExclusive(params string[] @params) => new MutuallyExclusive(@params);

        public static bool TryGetValue(this IEnumerable<Parameter> source, string key, out string? value)
        {
            var maybeParam = source.FirstOrDefault(p => p.Key.Equals(key));

            if (maybeParam is null)
            {
                value = null;
                return false;
            }
            else
            {
                if (maybeParam.Value is "")
                {
                    value = null;
                    return false;
                }
                value = maybeParam.Value;
                return true;
            }
        }
    }
}
