using System.Collections.Generic;

namespace Lib.Parsing.Mathematics
{
    public class GroupStack : Stack<List<Stack>>
    {
        public GroupStack(List<Stack> v)
            : base(v)
        {
        }
    }
}