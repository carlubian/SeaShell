using SeaShell.Core;
using System;

namespace SeaShell
{
    class Program
    {
        static void Main(string[] args)
        {
            // No arguments, run interactive shell
            if (args.Length is 0)
                SeaShellHost.Start();
            // Run args[0] as a script file
            else
                ;
        }
    }
}
