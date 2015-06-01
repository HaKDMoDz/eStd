using System.Parsers.Mathematics;

namespace System.Parsers.Mathematics
{
    public class VariableStack : Stack
    {
        public string Name;
        public object Value;

        public VariableStack(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}