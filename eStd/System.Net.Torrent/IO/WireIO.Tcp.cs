using System.Net.Sockets;

namespace System.Net.Torrent.IO
{
    public partial class WireIO
    {
        public class Tcp : IWireIO
        {
            private readonly Socket _socket;

            public int Timeout
            {
                get { return _socket.ReceiveTimeout / 1000; }
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

            public Tcp()
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.NoDelay = true;
            }

            public Tcp(Socket socket)
            {
                _socket = socket;

                if (_socket.AddressFamily != AddressFamily.InterNetwork)
                {
                    throw new ArgumentException("AddressFamily of socket must be InterNetwork");
                }

                if (_socket.SocketType != SocketType.Stream)
                {
                    throw new ArgumentException("SocketType of socket must be Stream");
                }

                if (_socket.ProtocolType != ProtocolType.Tcp)
                {
                    throw new ArgumentException("ProtocolType of socket must be Tcp");
                }

                _socket.NoDelay = true;
            }

            public void Connect(IPEndPoint endPoint)
            {
                _socket.Connect(endPoint);
            }

            public void Disconnect()
            {
                if (_socket.Connected)
                {
                    _socket.Disconnect(true);
                }
            }

            public int Send(byte[] bytes)
            {
                return _socket.Send(bytes);
            }

            public int Receive(ref byte[] bytes)
            {
                return _socket.Receive(bytes);
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
                _socket.Bind(ep);
                _socket.Listen(10);
            }

            public IWireIO Accept()
            {
                return new Tcp(_socket.Accept());
            }

            public IAsyncResult BeginAccept(AsyncCallback callback)
            {
                return _socket.BeginAccept(callback, this);
            }

            public IWireIO EndAccept(IAsyncResult ar)
            {
                return new Tcp(_socket.EndAccept(ar));
            }
        }
    }
}