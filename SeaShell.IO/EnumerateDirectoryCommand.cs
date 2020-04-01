using Pastel;
using SeaShell.Core;
using SeaShell.Core.Extensibility;
using SeaShell.Core.Extensibility.DuckTyping;
using SeaShell.Core.Extensibility.Parameters;
using SeaShell.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SeaShell.IO
{
    public class EnumerateDirectoryCommand : ISeaShellCommand
    {
        public string Name => "Enumerate-Directory";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Returns a list of files and folders in a directory.",
            Example = "Enumerate-Directory [/Target] DirName",
            Parameters = new Dictionary<string, string>
            {
                { "/Target (default)", "Specify the target directory. If missing, use current directory." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var dirName = Environment.CurrentDirectory;

            // Default parameter has a value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasNone("Target").Eval())
            {
                dirName = parameters.Single(p => p.Key == "_default").Value;
            }

            // Target parameter has a value
            if (Parameters.SeeIf(parameters).IsEmpty("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                dirName = parameters.Single(p => p.Key == "Target").Value;
            }

            // Both default and Target parameters have value
            if (Parameters.SeeIf(parameters).HasValue("_default").HasParam("Target")
                .HasValue("Target").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Target");
                return null;
            }

            if (!Directory.Exists(dirName))
            {
                SeaShellErrors.NotifyInvalidPath(dirName);
                return null;
            }

            var pipeRet = new List<EnumerateDirectoryPipelineObject>();

            ConsoleIO.WriteInfo($"Enumerating directory {dirName.Pastel("#F0F5FF")}:");
            ConsoleIO.WriteInfo("  Length    Name");

            foreach (var dir in Directory.EnumerateDirectories(dirName))
            {
                Console.WriteLine($"            {new DirectoryInfo(dir).Name}");
                pipeRet.Add(new EnumerateDirectoryPipelineObject()
                {
                    StringValue = new DirectoryInfo(dir).Name
                });
            }
            foreach (var file in Directory.EnumerateFiles(dirName))
            {
                var length = (int)new FileInfo(file).Length;
                Console.Write(length.ToString());

                for (int i = length.ToString().Length; i < 12; i++)
                    Console.Write(" ");

                Console.WriteLine(new FileInfo(file).Name);
                pipeRet.Add(new EnumerateDirectoryPipelineObject()
                {
                    StringValue = new FileInfo(file).Name
                });
            }

            return pipeRet;
        }
    }

    public class EnumerateDirectoryPipelineObject : IPipelinePrintable
    {
        public string StringValue { get; set; }
    }
}
