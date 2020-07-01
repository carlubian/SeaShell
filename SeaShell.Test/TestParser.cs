using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeaShell.Core.Grammar;
using SeaShell.Core.Model;
using Sprache;
using System;
using System.Linq;

namespace SeaShell.Test
{
    [TestClass]
    public class TestParser
    {
        [TestMethod]
        public void ParseIdent()
        {
            var cmd = "Exit";
            TestPositive();

            cmd = "Enumerate-Directory";
            TestPositive();

            cmd = "256-Encryption";
            TestPositive();

            cmd = "IPv6";
            TestPositive();

            cmd = "Fetch_File";
            TestNegative();

            cmd = "Help?";
            TestNegative();

            cmd = "&Query-Param";
            TestNegative();

            void TestPositive()
            {
                SeaShellParser.ident.Parse(cmd).Should().NotBeNull()
                    .And.Match(i => (i as Ident).Content == cmd);
            }

            void TestNegative()
            {
                Action act = () => SeaShellParser.ident.Parse(cmd);
                act.Should().Throw<ParseException>();
            }
        }

        [TestMethod]
        public void ParseAnyText()
        {
            var cmd = "Foo";
            TestPositive();

            cmd = "Two words";
            TestPositive();

            cmd = ".\\path\\file.txt";
            TestPositive();

            cmd = "attr = value";
            TestPositive();

            cmd = "badly/formatted/uri";
            // Only parses the first part
            SeaShellParser.anyText.Parse(cmd).Should().NotBeNull()
                    .And.Match(i => i.ToString() == "badly");

            cmd = "\"properly/formatted/uri\"";
            // Double quotes are excluded from parsing
            SeaShellParser.anyText.Parse(cmd).Should().NotBeNull()
                    .And.Match(i => i.ToString() == "properly/formatted/uri");

            void TestPositive()
            {
                SeaShellParser.anyText.Parse(cmd).Should().NotBeNull()
                    .And.Match(i => i.ToString() == cmd);
            }

            void TestNegative()
            {
                Action act = () => SeaShellParser.anyText.Parse(cmd);
                act.Should().Throw<ParseException>();
            }
        }

        [TestMethod]
        public void ParseParameter()
        {
            var key = "/Target";
            var value = ".\\Foo";
            TestPositive();

            key = "/Source-File";
            value = "Dataset.csv";
            TestPositive();

            key = "/Silent";
            value = "";
            TestPositive();

            key = "/Match-Predicate";
            value = "Type equals Folder";
            TestPositive();

            void TestPositive()
            {
                SeaShellParser.parameter.Parse($"{key} {value}").Should().NotBeNull()
                    .And.Match(i => (i as Parameter).Key == key.Replace("/", ""))
                    .And.Match(i => (i as Parameter).Value == value);
            }

            void TestNegative()
            {
                Action act = () => SeaShellParser.parameter.Parse($"{key} {value}");
                act.Should().Throw<ParseException>();
            }
        }

        [TestMethod]
        public void ParseCommand()
        {
            var cmd = "Copy-File";
            var @default = "";
            var @params = "/Source-File .\\tmp\\data.txt /Target-File .\\data\\data.txt";
            TestPositive();

            cmd = "Where";
            @default = "Length greater 16000";
            @params = "";
            TestPositive();

            cmd = "Enumerate-Directory";
            @default = ".";
            @params = "/Ignore-Subdirectories";
            TestPositive();

            cmd = "Fetch-Url";
            @params = "/Target \"www.contoso.com/foo/bar.html\"";
            // Double quotes are excluded from parsing
            SeaShellParser.command.Parse($"{cmd} {@params}").Should().NotBeNull()
                    .And.Match(c => (c as Command).Name == cmd)
                    .And.Match(c => (c as Command).Parameters.First().Value == "www.contoso.com/foo/bar.html");


            void TestPositive()
            {
                SeaShellParser.command.Parse($"{cmd} {@default} {@params}").Should().NotBeNull()
                    .And.Match(c => (c as Command).Name == cmd)
                    .And.Match(c => (c as Command).Parameters.Count() == @params.ToCharArray().Where(n => n == '/').Count() + 1);
            }
        }

        [TestMethod]
        public void ParsePipeline()
        {
            var query = "Exit";
            TestPositive();

            query = "Read-File message.txt > Print";
            TestPositive();

            query = "Enumerate-Directory /Source-Directory . > Where Type equals File > Print";
            TestPositive();

            void TestPositive()
            {
                SeaShellParser.pipeline.Parse(query).Should().NotBeNull()
                    .And.Match(p => (p as Pipeline).Commands.Count() == query.ToCharArray().Where(n => n == '>').Count() + 1);
            }
        }
    }
}
