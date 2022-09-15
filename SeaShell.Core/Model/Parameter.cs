namespace SeaShell.Core.Model
{
    public class Parameter
    {
        public Ident Key { get; set; }
        public string Value { get; set; }

        public Parameter(Ident key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString() => $"/{Key} {Value ?? ""}";
    }
}
