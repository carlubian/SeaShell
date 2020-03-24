# SeaShell Command Libraries

Libraries allow end users to import and use external commands
inside SeaShell, similarly to NuGet packages.

## File structure

```
LibraryFile.ssl
  Manifest.ini
  Assemblies
    ContosoCommands.dll
    Newtonsoft.Json.dll
```

* Library file: Root container. The SSL extension comes from SeaShell Library. It's actually a ZIP file but with the extension changed.
* Manifest.ini: Information about the library. See details below.
* Assemblies folder: Contains both the assembly(ies) with actual command implementations as well as any dependencies.

## Manifest file

All libraries require a manifest file to be valid.

```
[SeaShell]
Manifest Version = 1
Host Version = 0.6 - 1.1
Runtime Version = 3.1

[Library]
Name = Contoso.Network.API
Version = 1.0.16
Author = Buck L Up
URI = https://www.github.com/Contoso/SeaShellApiCommands
Description = Includes commands to interact with the Contoso API servers.
Assemblies = ContosoCommands.dll

[Dependencies]
Contoso.Core = 1.3.0
```

### SeaShell section

Contains generic information about version compatibility.

* Manifest Version: To support syntax changes in the manifest files.
* Host Version: Compatible versions of the SeaShell host itself.
* Runtime Version: .NET Core platform required by the assemblies.

<em>Idea? Version numbers can refer to a single version (2.3) or a range of versions (1.3 - 1.7)</em>

### Library section

Contains specific information about the library.

* Name: Unique name for the library. Just in case, no spaces allowed.
* Version: Current version of the library.
* Author: Name, email or other identification for the creator.
* URI: A website with information or other useful data about the library.
* Description: A short text explaining the purpose of the library.
* Assemblies: A comma separated list of files containing commands. Note: Don't include dependencies in this list.

### Dependencies section

Optional section indicating other SeaShell libraries that are required by this one.

Note that this section isn't intended to include generic .NET dependencies.