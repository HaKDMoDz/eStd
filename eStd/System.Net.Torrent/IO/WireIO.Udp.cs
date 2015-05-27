using System.Net.Sockets;

namespace System.Net.Torrent.IO
{
    public partial class WireIO
    {
        public class Udp : IWireIO
        {
            private readonly Socket _socket;
            private IPEndPoint _endPoint;

            public int Timeout
            {
                get
                {
                    return _socket.ReceiveTimeout / 1000;
                }
                set
                {
                    _socket.ReceiveTimeout = value * 1000;
                    _socket.SendTimeout = value * 1000;
                }
            }

            public bool Connected
            {
                get { return _socket.Connected; }
            }

            public Udp()
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }

            public void Connect(IPEndPoint endPoint)
            {
                _endPoint = endPoint;
            }

            public void Disconnect()
            {
                _socket.Disconnect(true);
            }

            public int Send(byte[] bytes)
            {
                return _socket.SendTo(bytes, _endPoint);
            }

            public int Receive(ref byte[] bytes)
            {
                EndPoint recFrom = new IPEndPoint(IPAddress.Any, _endPoint.Port);

                return _socket.ReceiveFrom(bytes, ref recFrom);
            }

            public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
            {
                return _socket.BeginReceive(buffer, offset, size, SocketFlags.None, callback, state);
            }

            public int EndReceive(IAsyncResult asyncResult)
            {
                return _socket.EndReceive(asyncResult);
            }

            public void Listen(EndPoint ep)
            {
                throw new NotImplementedException();
            }

            public IWireIO Accept()
            {
                throw new NotImplementedException();
            }

            public IAsyncResult BeginAccept(AsyncCallback callback)
            {
                throw new NotImplementedException();
            }

            public IWireIO EndAccept(IAsyncResult ar)
            {
                throw new NotImplementedException();
            }
        }
    }
}