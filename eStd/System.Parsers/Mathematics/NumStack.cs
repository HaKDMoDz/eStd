using Lib.Parsing.Mathematics;

namespace Creek.Parsers.Mathematics
{
    public class NumStack : Stack<double>
    {
        public NumStack(double v)
            : base(v)
        {
        }
    }
}