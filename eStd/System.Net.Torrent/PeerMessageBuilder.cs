using System.Collections.Generic;
using System.Net.Torrent.Misc;

namespace System.Net.Torrent
{
    public class PeerMessageBuilder : IDisposable
    {
        public UInt32 PacketLength { get { return (UInt32)(5 + MessagePayload.Count); } }
        public UInt32 MessageLength { get { return (UInt32)(1 + MessagePayload.Count); } }
        public byte MessageID { get; private set; }
        public List<byte> MessagePayload { get; private set; }

        public PeerMessageBuilder(byte msgId)
        {
            MessageID = msgId;

            MessagePayload = new List<byte>();
        }

        public PeerMessageBuilder Add(byte b)
        {
            MessagePayload.Add(b);

            return this;
        }

        public PeerMessageBuilder Add(byte[] bytes)
        {
            MessagePayload.AddRange(bytes);

            return this;
        }

        public PeerMessageBuilder Add(UInt32 n, Pack.Endianness endianness = Pack.Endianness.Big)
        {
            MessagePayload.AddRange(Pack.UInt32(n, endianness));

            return this;
        }

        public PeerMessageBuilder Add(String str)
        {
            MessagePayload.AddRange(Pack.Hex(str));

            return this;
        }

        public byte[] Message()
        {
            byte[] messageBytes = new byte[PacketLength];
            byte[] lengthBytes = Pack.UInt32(MessageLength, Pack.Endianness.Big);

            lengthBytes.CopyTo(messageBytes, 0);

            messageBytes[4] = MessageID;

            MessagePayload.CopyTo(messageBytes, 5);

            return messageBytes;
        }

        public void Dispose()
        {
            MessagePayload.Clear();
        }
    }
}