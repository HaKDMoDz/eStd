using System.Linq;
using System.Net.Torrent.BEncode;
using System.Net.Torrent.Misc;
using System.Net.Torrent.ProtocolExtensions;
using System.Text;

namespace System.Net.Torrent.Extensions
{
    public class UTMetadata : IBTExtension
    {
        private Int64 _metadataSize;
        private Int64 _pieceCount;
        private Int64 _piecesReceived;
        private byte[] _metadataBuffer;
        private ExtendedProtocolExtensions _parent;

        public string Protocol
        {
            get { return "ut_metadata"; }
        }

        public event Action<IPeerWireClient, IBTExtension, BDict> MetaDataReceived;

        public void Init(ExtendedProtocolExtensions parent)
        {
            _parent = parent;
            _metadataBuffer = new byte[0];
        }

        public void Deinit()
        {

        }

        public void OnHandshake(IPeerWireClient peerWireClient, byte[] handshake)
        {
            BDict dict = (BDict)BencodingUtils.Decode(handshake);
            if (dict.ContainsKey("metadata_size"))
            {
                BInt size = (BInt)dict["metadata_size"];
                _metadataSize = size;
                _pieceCount = (Int64)Math.Ceiling((double)_metadataSize / 16384);
            }

            RequestMetaData(peerWireClient);
        }

        public void OnExtendedMessage(IPeerWireClient peerWireClient, byte[] bytes)
        {
            Int32 startAt = 0;
            BencodingUtils.Decode(bytes, ref startAt);
            _piecesReceived += 1;

            if (_pieceCount >= _piecesReceived)
            {
                _metadataBuffer = _metadataBuffer.Concat(bytes.Skip(startAt)).ToArray();
            }

            if (_pieceCount == _piecesReceived)
            {
                BDict metadata = (BDict)BencodingUtils.Decode(_metadataBuffer);

                if (MetaDataReceived != null)
                {
                    MetaDataReceived(peerWireClient, this, metadata);
                }
            }
        }

        public void RequestMetaData(IPeerWireClient peerWireClient)
        {
            byte[] sendBuffer = new byte[0];

            for (Int32 i = 0; i < _pieceCount; i++)
            {
                BDict masterBDict = new BDict
                {
                    {"msg_type", (BInt) 0}, 
                    {"piece", (BInt) i}
                };

                String encoded = BencodingUtils.EncodeString(masterBDict);

                byte[] buffer = Pack.Int32(2 + encoded.Length, Pack.Endianness.Big);
                buffer = buffer.Concat(new byte[] {20}).ToArray();
                buffer = buffer.Concat(new[] { _parent.GetOutgoingMessageID(peerWireClient, this) }).ToArray();
                buffer = buffer.Concat(Encoding.GetEncoding(1252).GetBytes(encoded)).ToArray();

                sendBuffer = sendBuffer.Concat(buffer).ToArray();
            }

            peerWireClient.SendBytes(sendBuffer);
        }

    }
}