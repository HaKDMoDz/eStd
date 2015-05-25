using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using Rpc.Internals;

namespace Rpc
{
    public class RpcClient
    {
        public RpcClient(string url)
        {
            this.Url = url;
        }


        public dynamic CreateProxy<T>()
            where T : IRpcProxy
        {
            Type t = typeof(T);
            
            return new Proxy(typeof(T), this, typeof(T).Name);
        }

        public string Url { get; private set; }

        public object Call(string methodName, params object[] parameters)
        {
            XmlRpcRequest client = new XmlRpcRequest(methodName, parameters);

            XmlRpcResponse response = client.Send(Url);

            return response.Value;
        }
    }
}