# Create a SeaShell command library

Command libraries are a way to extend the SeaShell functionality with new commands.

## Create the command code

Any .NET project can act as a command library, but this function is normally done by
class libraries. 

Note that, in any case, the project can't target netstandard2.0. It must target
instead netcoreapp3.1 (or presumably net5 once it's released).

It will also require a reference to SeaShell.Core.

Once that is taken care of, create a new class and make it implement the ISeaShellCommand
interface:

```csharp
using SeaShell.Core.Extensibility;
using System;

namespace CustomLibrary
{
    public class SayHelloCommand : ISeaShellCommand
    {

    }
}
```

The ISeaShellCommand interface includes the following required implementations:

* string Name: The name used to invoke the command from the shell. Standard convention is to use capitalized words separated by dashes, like Command, Command-Name or Long-Command-Name.
* CommandHelp Help: A structure containing help information that will be viewed when invoking the built-in Help command on your custom command.
* IEnumerable&lt;dynamic&gt; Invoke(IEnumerable&lt;Parameter&gt; parameters, IEnumerable&lt;dynamic&gt; pipeline): The main invocation function.

A reference implementation of the above requirements is as follows:

```csharp
using SeaShell.Core.Extensibility;
using System;

namespace CustomLibrary
{
    public class SayHelloCommand : ISeaShellCommand
    {
        public string Name => "Say-Hello";

        public CommandHelp Help => new CommandHelp
        {
            Description = "Greets the user.",
            Example = "Say-Hello [[/Name] User name]",
            Parameters = new Dictionary<string, string>
            {
                { "/Name (default)", "The name of the user." }
            }
        };

        public IEnumerable<dynamic> Invoke(IEnumerable<Parameter> parameters, IEnumerable<dynamic> pipeline)
        {
            var name = "";

            // Default parameter and Target parameter present
            if (Parameters.SeeIf(parameters).HasParam("_default").HasParam("Name").Eval())
            {
                SeaShellErrors.NotifyMutuallyExclusive("_default", "Name");
                return null;
            }

            // Target parameter without value
            if (Parameters.SeeIf(parameters).HasParam("Name").IsEmpty("Name").Eval())
            {
                SeaShellErrors.NotifyParamMissingValue("Name");
                return null;
            }

            // Default parameter with value
            if (Parameters.SeeIf(parameters).HasValue("_default").Eval())
            {
                name = parameters.Single(p => p.Key == "_default").Value;
            }

            // Target parameter with value
            if (Parameters.SeeIf(parameters).HasParam("Name").HasValue("Name").Eval())
            {
                name = parameters.Single(p => p.Key == "Name").Value;
            }

            if (name is "")
                Console.WriteLine("Hello.");
            else
                Console.WriteLine($"Hello, {name}.");

            return null;
        }
    }
}
```

TODO explain the reference implementation.

## Build and package the library

Compile the project and copy the resulting DLL and all its dependencies to an empty directory.

Note that the SeaShell.Core.dll dependency should not be included, as it will automatically be
included by the shell host.

Next, create a manifest file, either manually or running Otter /Create on this directory.

Change the necessary fields on the manifest, and move all assemblies to an Assemblies subdirectory.

Now, run the Otter /Pack on the directory containing the manifest file to obtain a SSL library file.

## Use the library

Start a SeaShell session and navigate to the directory containing the SSL file. Then run the
Otter /Install command on it.

The library will now be installed in the global command store. Future shell sessions will be
able to use it without installing again.

## Miscellaneous notes

Multiple command libraries can use different versions of the same assembly dependency (for example,
Newtonsoft.json 10.0 and Newtonsoft.Json 11.0), as each library is loaded into a different context.
