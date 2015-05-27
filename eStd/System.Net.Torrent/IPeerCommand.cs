namespace System.Net.Torrent
{
    public interface IPeerCommand
    {
        Int32 Length { get; set; }
        byte CommandID { get; set; }
        byte[] Payload { get; set; }
    }
}