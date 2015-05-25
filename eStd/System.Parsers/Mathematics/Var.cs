using Lib.Parsing.Mathematics;

namespace Creek.Parsers.Mathematics
{
    public class Var : Stack
    {
        public string Name;
        public object Value;

        public Var(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
