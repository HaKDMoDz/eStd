using System;
using System.Linq;
using Rpc.Internals;

namespace System.Communication.XmlRpc
{
    public class RpcServer
    {
        public RpcServer(int port)
        {
            this.port = port;
            server = new XmlRpcServer(port);
        }

        public void AddListener(string name, object obj)
        {
            server.Add(name, obj);
        }

        public void AddListener(object obj)
        {
            server.Add(obj.GetType().Name, obj);
        }

        private readonly int port;
        private readonly XmlRpcServer server;

        public void Start()
        {
            server.Start();
        }
    }
}