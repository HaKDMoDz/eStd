namespace System.Net.Torrent.ProtocolExtensions
{
    public interface IBTExtension
    {
        String Protocol { get; }
        void Init(ExtendedProtocolExtensions parent);
        void Deinit();
        void OnHandshake(IPeerWireClient peerWireClient, byte[] handshake);
        void OnExtendedMessage(IPeerWireClient peerWireClient, byte[] bytes);
    }
}