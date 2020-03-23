using DotNet.Misc.Extensions.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Extensibility
{
    public static class SeaShellErrors
    {
        internal static bool CheckLowercaseCommands(string cmdName)
        {
            if (cmdName is "exit")
            {
                ConsoleIO.WriteWarning("For future reference, the Exit command starts with an uppercase letter.");
                SeaShellHost.Continue = false;
                return true;
            }
            if (cmdName is "help")
            {
                ConsoleIO.WriteWarning("The correct usage of Help is to type Help [Command-Name].");
                ConsoleIO.WriteWarning("Use List-Commands for a list of all available commands.");
                return true;
            }

            return false;
        }

        public static void NotifyUnknownCommand(string cmdName)
        {
            ConsoleIO.WriteError($"The command {cmdName} is not recognized.");
            ConsoleIO.WriteWarning("  If that command comes from an external source, make sure it's correctly downloaded and installed.");

            if (char.IsLower(cmdName[0]))
                ConsoleIO.WriteWarning("  Also remember that most official commands start with an uppercase letter.");
        }

        public static void NotifyParamMissingValue(string param)
        {
            ConsoleIO.WriteError($"The parameter {param} requires a value.");
        }

        public static void NotifyMissingParam(string param)
        {
            ConsoleIO.WriteError($"The parameter {param} must be present.");
        }

        public static void NotifyMissingOneOfParams(params string[] oneOf)
        {
            ConsoleIO.WriteError($"One of the following parameters must be present: {oneOf.Stringify(p => p, ", ")}.");
        }

        public static void NotifyMutuallyExclusive(params string[] onlyOne)
        {
            ConsoleIO.WriteError($"Only one of the following parameters can appear: {onlyOne.Stringify(p => p, ", ")}.");
        }
    }
}
