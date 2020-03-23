using SeaShell.Core.Extensibility;
using SeaShell.Core.Grammar;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SeaShell.Test")]
namespace SeaShell.Core
{
    public static class SeaShellHost
    {
        internal static readonly string Version = "0.1.0.230320";
        internal static bool Continue = true;

        public static void Start()
        {
            ConsoleIO.Initialize();
            Commands.PopulateSystemCommands();
            Commands.PopulateGlobalCommands();
            Commands.PopulateLocalCommands();

            var oldDir = Environment.CurrentDirectory;

            while (Continue)
            {
                ConsoleIO.ShowPrompt();
                var pipeline = SeaShellParser.pipeline.Parse(Console.ReadLine());

                IEnumerable<dynamic> lastReturn = null;
                foreach (var command in pipeline.Commands)
                {
                    if (SeaShellErrors.CheckLowercaseCommands(command.Name))
                        continue;
                    var handler = Commands.HandlerFor(command.Name);
                    if (handler is null)
                    {
                        SeaShellErrors.NotifyUnknownCommand(command.Name);
                        break;
                    }
                    if (lastReturn != null)
                        command.PipelineParameter = lastReturn;
                    lastReturn = handler.Invoke(command.Parameters, lastReturn);
                }
            }

            Environment.CurrentDirectory = oldDir;
            ConsoleIO.Restore();
        }
    }
}
