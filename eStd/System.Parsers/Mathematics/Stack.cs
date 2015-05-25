namespace Lib.Parsing.Mathematics
{
    public abstract class Stack<t> : Stack
    {
        public t Value { get; set; }
        protected Stack(t v)
        {
            Value = v;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public abstract class Stack
    {
    }
}