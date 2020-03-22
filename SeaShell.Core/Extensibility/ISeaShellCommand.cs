using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility
{
    public interface ISeaShellCommand
    {
        string Name { get; }
        CommandHelp Help { get; }

        IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline);
    }
}
