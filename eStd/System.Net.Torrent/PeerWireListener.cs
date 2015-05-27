using System.Net.Torrent.IO;

namespace System.Net.Torrent
{
    public class PeerWireListener
    {
        private readonly IWireIO _socket;
        private readonly int _port;
        private IAsyncResult _ar = null;

        public event Action<PeerWireClient> NewPeer;

        public PeerWireListener(int port)
        {
            _port = port;
            _socket = new WireIO.Tcp();
        }

        public void StartListening()
        {
            _socket.Listen(new IPEndPoint(IPAddress.Any, _port));
            _ar = _socket.BeginAccept(Callback);
        }


        public void StopListening()
        {
            if (_ar != null)
            {
                _socket.EndAccept(_ar);
            }
        }

        private void Callback(IAsyncResult ar)
        {
            IWireIO s = _socket.EndAccept(ar);

            if (NewPeer != null)
            {
                NewPeer(new PeerWireClient(s));
            }

            _socket.BeginAccept(Callback);
        }
    }
}