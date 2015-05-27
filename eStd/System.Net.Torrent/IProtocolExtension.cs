namespace System.Net.Torrent
{
    public interface IProtocolExtension
    {
        byte[] ByteMask { get; }
        byte[] CommandIDs { get; }
        bool OnHandshake(IPeerWireClient client);
        bool OnCommand(IPeerWireClient client, Int32 commandLength, byte commandId, byte[] payload);
    }
}