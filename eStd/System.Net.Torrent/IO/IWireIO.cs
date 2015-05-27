namespace System.Net.Torrent.IO
{
    public interface IWireIO
    {
        int Timeout { get; set; }
        bool Connected { get; }

        void Connect(IPEndPoint endPoint);
        void Disconnect();
        int Send(byte[] bytes);
        int Receive(ref byte[] bytes);
        IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state);
        int EndReceive(IAsyncResult asyncResult);

        void Listen(EndPoint ep);
        IWireIO Accept();
        IAsyncResult BeginAccept(AsyncCallback callback);
        IWireIO EndAccept(IAsyncResult ar);
    }
}