using System;
using System.Collections.Generic;
using System.Text;

namespace SeaShell.Core.Model
{
    public class Ident
    {
        public string Content { get; set; }

        public Ident(string value)
        {
            Content = value;
        }

        public override string ToString()
        {
            return Content.ToString();
        }

        public override bool Equals(object? obj) => Content.Equals(obj);

        public override int GetHashCode() => Content.GetHashCode();

        public static implicit operator string(Ident source) => source.Content;

        public static implicit operator Ident(string source) => new Ident(source);
    }
}
