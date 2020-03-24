using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeaShell.Core.Libraries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Test
{
    [TestClass]
    public class TestManifest
    {
        [TestMethod]
        public void ParseManifest()
        {
            var manifest = Manifest.Parse("Manifest.ini");

            manifest.ManifestVersion.Should().Be("1");
            manifest.HostVersion.Should().Be("0.6 - 1.1");
            manifest.RuntimeVersion.Should().Be("3.1");

            manifest.Name.Should().Be("Contoso.Network.API");
            manifest.LibraryVersion.Should().Be("1.0.16");
            manifest.Author.Should().Be("Buck L Up");
            manifest.URI.Should().Be("https://www.github.com/Contoso/SeaShellApiCommands");
            manifest.Description.Should().Be("Includes commands to interact with the Contoso API servers.");
            manifest.Assemblies.Should().HaveCount(1)
                .And.ContainSingle("ContosoCommands.dll");
        }
    }
}
