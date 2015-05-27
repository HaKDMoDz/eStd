using System;
using System.Communication;
using System.Dynamic;
using System.Linq;

namespace System.Communication.XmlRpc
{
    public class Proxy : DynamicObject
    {
        public Proxy()
        {

        }

        private readonly RpcClient p;

        private readonly string servername;

        public Proxy(Type t, RpcClient p, string servername)
        {
            this.T = t;
            this.p = p;
            this.servername = servername;
        }

        public Type T { get; private set; }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = p.Call(servername + "." + binder.Name, args);

            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == T)
            {
                result = Activator.CreateInstance(T);

                return true;
            }
            // your other conversions
            return base.TryConvert(binder, out result);
        }
    }
}