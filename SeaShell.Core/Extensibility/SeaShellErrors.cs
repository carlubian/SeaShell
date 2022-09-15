using SeaShell.Core.Extensibility.Parameters;

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

        public static void NotifyParamValuePresent(string param)
        {
            ConsoleIO.WriteError($"The parameter {param} cannot have a value.");
        }

        public static void NotifyMissingParam(string param)
        {
            ConsoleIO.WriteError($"The parameter {param} must be present.");
        }

        public static void NotifyParamPresent(string param)
        {
            ConsoleIO.WriteError($"The parameter {param} cannot be present.");
        }

        public static void NotifyMissingOneOfParams(params string[] oneOf)
        {
            ConsoleIO.WriteError($"One of the following parameters must be present: {string.Join(',', oneOf)}.");
        }

        public static void NotifyMutuallyExclusive(params string[] onlyOne)
        {
            ConsoleIO.WriteError($"Only one of the following parameters can appear: {string.Join(',', onlyOne)}.");
        }

        public static void NotifyInvalidPath(string path)
        {
            ConsoleIO.WriteError($"The path {path} is invalid, nonexistant or unreachable.");
        }

        public static void NotifyOrParamError(IParameterCheck[] parts)
        {
            // TODO check parts and print detailed information
            ConsoleIO.WriteError($"Parameter verification error.");
        }

        public static void NotifyAndParamError(IParameterCheck[] parts)
        {
            // TODO check parts and print detailed information
            ConsoleIO.WriteError($"Parameter verification error.");
        }
    }
}
