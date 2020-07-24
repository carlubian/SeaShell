using SeaShell.Core.Extensibility;
using SeaShell.Core.Grammar;
using Sprache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("SeaShell.Test")]
[assembly: InternalsVisibleTo("SeaShell.Otter")]
[assembly: InternalsVisibleTo("SeaShell.Reflection")]
namespace SeaShell.Core
{
    public static class SeaShellHost
    {
        internal static readonly string Version = "0.6.1.240720";
        internal static bool Continue = true;
        internal static string Env = "_system";
        internal static string EnvPath = "";

        internal static IDictionary<string, string> Variables = new LenientDictionary<string, string>();

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
                ParseAndRun(Console.ReadLine());
            }

            Environment.CurrentDirectory = oldDir;
            ConsoleIO.Restore();
        }

        public static void ParseAndRun(string text)
        {
            var pipeline = SeaShellParser.pipeline.Parse(text);

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
