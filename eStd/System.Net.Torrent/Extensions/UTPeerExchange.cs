using System.Net.Torrent.BEncode;
using System.Net.Torrent.Misc;
using System.Net.Torrent.ProtocolExtensions;

namespace System.Net.Torrent.Extensions
{
    public class UTPeerExchange : IBTExtension
    {
        private ExtendedProtocolExtensions _parent;

        public string Protocol
        {
            get { return "ut_pex"; }
        }

        public event Action<IPeerWireClient, IBTExtension, IPEndPoint, byte> Added;
        public event Action<IPeerWireClient, IBTExtension, IPEndPoint> Dropped;

        public void Init(ExtendedProtocolExtensions parent)
        {
            _parent = parent;
        }

        public void Deinit()
        {

        }

        public void OnHandshake(IPeerWireClient peerWireClient, byte[] handshake)
        {
            BDict d = (BDict)BencodingUtils.Decode(handshake);
        }

        public void OnExtendedMessage(IPeerWireClient peerWireClient, byte[] bytes)
        {
            BDict d = (BDict) BencodingUtils.Decode(bytes);
            if (d.ContainsKey("added") && d.ContainsKey("added.f"))
            {
                BString pexList = (BString)d["added"];
                BString pexFlags = (BString)d["added.f"];

                for (int i = 0; i < pexList.ByteValue.Length/6; i++)
                {
                    UInt32 ip = Unpack.UInt32(pexList.ByteValue, i*6, Unpack.Endianness.Little);
                    UInt16 port = Unpack.UInt16(pexList.ByteValue, (i * 6) + 4, Unpack.Endianness.Big);
                    byte flags = pexFlags.ByteValue[i];

                    IPEndPoint ipAddr = new IPEndPoint(ip, port);

                    if (Added != null)
                    {
                        Added(peerWireClient, this, ipAddr, flags);
                    }
                }
            }

            if (d.ContainsKey("dropped"))
            {
                BString pexList = (BString)d["dropped"];

                for (int i = 0; i < pexList.ByteValue.Length / 6; i++)
                {
                    UInt32 ip = Unpack.UInt32(pexList.ByteValue, i * 6, Unpack.Endianness.Little);
                    UInt16 port = Unpack.UInt16(pexList.ByteValue, (i * 6) + 4, Unpack.Endianness.Big);

                    IPEndPoint ipAddr = new IPEndPoint(ip, port);

                    if (Dropped != null)
                    {
                        Dropped(peerWireClient, this, ipAddr);
                    }
                }
            }
        }

        public void SendMessage(IPeerWireClient peerWireClient, IPEndPoint[] addedEndPoints, byte[] flags, IPEndPoint[] droppedEndPoints)
        {
            if (addedEndPoints == null && droppedEndPoints == null) return;

            BDict d = new BDict();

            if (addedEndPoints != null)
            {
                byte[] added = new byte[addedEndPoints.Length * 6];
                for (int x = 0; x < addedEndPoints.Length; x++)
                {
                    addedEndPoints[x].Address.GetAddressBytes().CopyTo(added, x * 6);
                    BitConverter.GetBytes((ushort)addedEndPoints[x].Port).CopyTo(added, (x * 6)+4);
                }

                d.Add("added", new BString { ByteValue = added });
            }

            if (droppedEndPoints != null)
            {
                byte[] dropped = new byte[droppedEndPoints.Length * 6];
                for (int x = 0; x < droppedEndPoints.Length; x++)
                {
                    droppedEndPoints[x].Address.GetAddressBytes().CopyTo(dropped, x * 6);

                    dropped.SetValue((ushort)droppedEndPoints[x].Port, (x * 6) + 2);
                }

                d.Add("dropped", new BString { ByteValue = dropped });
            }

            _parent.SendExtended(peerWireClient, _parent.GetOutgoingMessageID(peerWireClient, this), BencodingUtils.EncodeBytes(d));
        }
    }
}