using SeaShell.Core.Model;
using System.Collections.Generic;

namespace SeaShell.Core.Extensibility
{
    public interface ISeaShellCommand
    {
        string Name { get; }
        CommandHelp Help { get; }

        IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline);
    }
}
