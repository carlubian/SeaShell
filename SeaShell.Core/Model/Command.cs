using DotNet.Misc.Extensions.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Model
{
    public class Command
    {
        public Ident Name { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }
        public string DefaultParameter
        {
            set
            {
                (Parameters as IList<Parameter>)?.Add(new Parameter("_default", value));
            }
        }
        public IEnumerable<dynamic> PipelineParameter { get; internal set; }

        public Command(Ident name, IEnumerable<Parameter> parameters)
        {
            Name = name;
            Parameters = parameters;
            PipelineParameter = Enumerable.Empty<dynamic>();
        }

        public override string ToString() => $"{Name} {Parameters.Stringify(p => p.ToString(), " ")}";
    }
}
