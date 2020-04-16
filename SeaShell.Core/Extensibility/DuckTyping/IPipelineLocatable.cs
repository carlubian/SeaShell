using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility.DuckTyping
{
    public interface IPipelineLocatable
    {
        string URI { get; }
    }
}
