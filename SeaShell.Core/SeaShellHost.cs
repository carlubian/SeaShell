using SeaShell.Core.Extensibility;
using SeaShell.Core.Grammar;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SeaShell.Test")]
namespace SeaShell.Core
{
    public static class SeaShellHost
    {
        internal static readonly string Version = "0.4.0.080620";
        internal static bool Continue = true;
        internal static string Env = "_system";
        internal static string EnvPath = "";

        public static void Start()
        {
            InitializeFolders();
            ConsoleIO.Initialize();
            Commands.PopulateSystemCommands();
            Commands.PopulateGlobalCommands();

            var oldDir = Environment.CurrentDirectory;

            while (Continue)
            {
                ConsoleIO.ShowPrompt();
                var pipeline = SeaShellParser.pipeline.Parse(Console.ReadLine());

                IEnumerable<dynamic> lastReturn = Enumerable.Empty<dynamic>();
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
                    if (!lastReturn.Any())
                        command.PipelineParameter = lastReturn;
                    lastReturn = handler.Invoke(command.Parameters, lastReturn);
                }
            }

            Environment.CurrentDirectory = oldDir;
            ConsoleIO.Restore();
        }

        private static void InitializeFolders()
        {
            var baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".SeaShell");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            var libDir = Path.Combine(baseDir, "Libraries");
            if (!Directory.Exists(libDir))
                Directory.CreateDirectory(libDir);
        }
    }
}
