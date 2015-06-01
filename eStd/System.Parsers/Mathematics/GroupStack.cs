using System.Collections.Generic;

namespace System.Parsers.Mathematics
{
    public class GroupStack : System.Parsers.Mathematics.Stack<List<Stack>>
    {
        public GroupStack(List<System.Parsers.Mathematics.Stack> v) : base(v)
        {
        }
    }
}