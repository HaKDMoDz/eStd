using Lib.Parsing.Mathematics;

namespace Creek.Parsers.Mathematics
{
    public class OpStack : Stack<string>
    {
        public OpStack(string v)
            : base(v)
        {
        }
    }
}