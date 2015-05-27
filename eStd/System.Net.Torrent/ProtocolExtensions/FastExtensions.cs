using System.Net.Torrent.Misc;

namespace System.Net.Torrent.ProtocolExtensions
{
    public class FastExtensions : IProtocolExtension
    {
        public event Action<IPeerWireClient, Int32> SuggestPiece;
        public event Action<IPeerWireClient, Int32, Int32, Int32> Reject;
        public event Action<IPeerWireClient> HaveAll;
        public event Action<IPeerWireClient> HaveNone;
        public event Action<IPeerWireClient, Int32> AllowedFast;

        public byte[] ByteMask
        {
            get { return new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04}; }
        }

        public byte[] CommandIDs
        {
            get 
            { 
                return new byte[]
                {
                    13,//Suggest Piece
                    14,//Have all
                    15,//Have none
                    16,//Reject Request
                    17 //Allowed Fast
                }; 
            }
        }

        public bool OnHandshake(IPeerWireClient client)
        {
            return false;
        }

        public bool OnCommand(IPeerWireClient client, int commandLength, byte commandId, byte[] payload)
        {
            switch (commandId)
            {
                case 13:
                    ProcessSuggest(client, payload);
                    break;
                case 14:
                    OnHaveAll(client);
                    break;
                case 15:
                    OnHaveNone(client);
                    break;
                case 16:
                    ProcessReject(client, payload);
                    break;
                case 17:
                    ProcessAllowFast(client, payload);
                    break;
            }

            return false;
        }

        private void ProcessSuggest(IPeerWireClient client, byte[] payload)
        {
            Int32 index = Unpack.Int32(payload, 0, Unpack.Endianness.Big);

            OnSuggest(client, index);
        }

        private void ProcessAllowFast(IPeerWireClient client, byte[] payload)
        {
            Int32 index = Unpack.Int32(payload, 0, Unpack.Endianness.Big);

            OnAllowFast(client, index);
        }

        private void ProcessReject(IPeerWireClient client, byte[] payload)
        {
            Int32 index = Unpack.Int32(payload, 0, Unpack.Endianness.Big);
            Int32 begin = Unpack.Int32(payload, 4, Unpack.Endianness.Big);
            Int32 length = Unpack.Int32(payload, 8, Unpack.Endianness.Big);

            OnReject(client, index, begin, length);
        }

        private void OnSuggest(IPeerWireClient client, Int32 pieceIndex)
        {
            if (SuggestPiece != null)
            {
                SuggestPiece(client, pieceIndex);
            }
        }

        private void OnHaveAll(IPeerWireClient client)
        {
            if (HaveAll != null)
            {
                HaveAll(client);
            }
        }

        private void OnHaveNone(IPeerWireClient client)
        {
            if (HaveNone != null)
            {
                HaveNone(client);
            }
        }

        private void OnReject(IPeerWireClient client, Int32 index, Int32 begin, Int32 length)
        {
            if (Reject != null)
            {
                Reject(client, index, begin, length);
            }
        }

        private void OnAllowFast(IPeerWireClient client, Int32 pieceIndex)
        {
            if (AllowedFast != null)
            {
                AllowedFast(client, pieceIndex);
            }
        }
    }
}