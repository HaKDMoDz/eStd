using System.Linq.Expressions;

namespace System.Data.Access
{
    class Lambda
    {
        public LambdaExpression callexp;
        public object p;

        public Lambda(LambdaExpression callexp, object p)
        {
            // TODO: Complete member initialization
            this.callexp = callexp;
            this.p = p;
        }

    }
}