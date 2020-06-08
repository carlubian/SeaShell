using SeaShell.Core.Model;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaShell.Core.Grammar
{
    internal static class ScriptParser
    {
        /// <summary>
        /// Ident: Letters, digits and dashes
        /// 
        /// e.g.:
        /// Print
        /// Enumerate-Directory
        /// Convert2Csv
        /// 256-Password
        /// </summary>
        internal static Parser<Ident> ident = from leading in Parse.WhiteSpace.Many()
                                              from content in Parse.LetterOrDigit.Or(
                                                  Parse.Char('-')).Many().Text()
                                              from trailing in Parse.WhiteSpace.AtLeastOnce().Or(
                                                  Parse.LineTerminator)
                                              select new Ident(content);

        /// <summary>
        /// AnyText: Used primarily as param
        /// value. Anything except a new param
        /// or the pipeline symbol.
        /// </summary>
        internal static Parser<string> anyText = from leading in Parse.WhiteSpace.Many()
                                                 from name in Parse.CharExcept("/>").Many().Text()
                                                 from trailing in Parse.WhiteSpace.Many()
                                                 select name.Trim();

        /// <summary>
        /// Parameter: Slash introduced Ident, then
        /// an optional AnyText as value.
        /// </summary>
        internal static Parser<Parameter> parameter = from leading in Parse.WhiteSpace.Many()
                                                      from symbol in Parse.Chars('/').Once()
                                                      from name in ident
                                                      from value in anyText.Optional()
                                                      from trailing in Parse.WhiteSpace.Many()
                                                      select new Parameter(name, value.GetOrElse(""));

        /// <summary>
        /// Command: Ident name, optional default parameter
        /// without name, and optional list of name-value parameters.
        /// </summary>
        internal static Parser<Command> command = from leading in Parse.WhiteSpace.Many()
                                                  from name in ident
                                                  from @default in anyText.Optional()
                                                  from @params in parameter.Many().Optional()
                                                  from trailing in Parse.WhiteSpace.Many()
                                                  select new Command(name, @params.GetOrElse(null))
                                                  {
                                                      DefaultParameter = @default.GetOrElse("")
                                                  };

        /// <summary>
        /// Pipeline: One or more Commands separated by '>'
        /// </summary>
        internal static Parser<Pipeline> pipeline = from leading in Parse.WhiteSpace.Many()
                                                    from first in command.Once()
                                                    from rest in (
                                                        from trailing in Parse.WhiteSpace.Many()
                                                        from separator in Parse.Char('>')
                                                        from infix in Parse.WhiteSpace.Many()
                                                        from command in command
                                                        select command).Many()
                                                    from trailing in Parse.WhiteSpace.Many()
                                                    select new Pipeline(rest.Prepend(first.First()));
    }
}
