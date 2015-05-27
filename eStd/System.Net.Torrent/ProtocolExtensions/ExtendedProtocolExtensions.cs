using System.Collections.Generic;
using System.Net.Torrent.BEncode;
using System.Net.Torrent.Misc;
using System.Text;

namespace System.Net.Torrent.ProtocolExtensions
{
    public class ExtendedProtocolExtensions : IProtocolExtension
    {
        public class ClientProtocolIDMap
        {
            public ClientProtocolIDMap(IPeerWireClient client, String protocol, byte commandId)
            {
                Client = client;
                Protocol = protocol;
                CommandID = commandId;
            }

            public IPeerWireClient Client { get; set; }
            public String Protocol { get; set; }
            public byte CommandID { get; set; }
        }

        private readonly List<IBTExtension> _protocolExtensions;
        private readonly List<ClientProtocolIDMap> _extOutgoing;
        private readonly List<ClientProtocolIDMap> _extIncoming;

        public ExtendedProtocolExtensions()
        {
            _protocolExtensions = new List<IBTExtension>();
            _extOutgoing = new List<ClientProtocolIDMap>();
            _extIncoming = new List<ClientProtocolIDMap>();
        }

        public byte[] ByteMask
        {
            get { return new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00 }; }
        }

        public byte[] CommandIDs
        {
            get { 
                return new byte[]
                {
                    20 //extended protocol
                }; 
            }
        }

        public bool OnHandshake(IPeerWireClient client)
        {
            BDict handshakeDict = new BDict();
            BDict mDict = new BDict();
            byte i = 1;
            foreach (IBTExtension extension in _protocolExtensions)
            {
                _extOutgoing.Add(new ClientProtocolIDMap(client, extension.Protocol, i));
                mDict.Add(extension.Protocol, new BInt(i));
                i++;
            }

            handshakeDict.Add("m", mDict);

            String handshakeEncoded = BencodingUtils.EncodeString(handshakeDict);
            byte[] handshakeBytes = Encoding.ASCII.GetBytes(handshakeEncoded);
            Int32 length = 2 + handshakeBytes.Length;

            client.SendBytes((new byte[0]).Cat(Pack.Int32(length, Pack.Endianness.Big).Cat(new[] { (byte)20 }).Cat(new[] { (byte)0 }).Cat(handshakeBytes)));

            return true;
        }

        public bool OnCommand(IPeerWireClient client, int commandLength, byte commandId, byte[] payload)
        {
            if (commandId == 20)
            {
                ProcessExtended(client, commandLength, payload);
                return true;
            }

            return false;
        }

        public void RegisterProtocolExtension(IPeerWireClient client, IBTExtension extension)
        {
            _protocolExtensions.Add(extension);
            extension.Init(this);
        }

        public void UnregisterProtocolExtension(IPeerWireClient client, IBTExtension extension)
        {
            _protocolExtensions.Remove(extension);
            extension.Deinit();
        }

        public byte GetOutgoingMessageID(IPeerWireClient client, IBTExtension extension)
        {
            ClientProtocolIDMap map = _extOutgoing.Find(f => f.Client == client && f.Protocol == extension.Protocol);

            if (map != null)
            {
                return map.CommandID;
            }

            return 0;
        }

        public bool SendExtended(IPeerWireClient client, byte extMsgId, byte[] bytes)
        {
            return client.SendBytes(new PeerMessageBuilder(20).Add(extMsgId).Add(bytes).Message());
        }

        private IBTExtension FindIBTExtensionByProtocol(String protocol)
        {
            foreach (IBTExtension protocolExtension in _protocolExtensions)
            {
                if (protocolExtension.Protocol == protocol)
                {
                    return protocolExtension;
                }
            }

            return null;
        }

        private String FindIBTProtocolByMessageID(int messageId)
        {
            foreach (ClientProtocolIDMap map in _extIncoming)
            {
                if (map.CommandID == messageId)
                {
                    return map.Protocol;
                }
            }

            return null;
        }

        private void ProcessExtended(IPeerWireClient client, int commandLength, byte[] payload)
        {
            Int32 msgId = payload[0];

            byte[] buffer = payload.GetBytes(1, commandLength - 1);

            if (msgId == 0)
            {
                BDict extendedHandshake = (BDict)BencodingUtils.Decode(buffer);

                BDict mDict = (BDict)extendedHandshake["m"];
                foreach (KeyValuePair<string, IBencodingType> pair in mDict)
                {
                    BInt i = (BInt)pair.Value;
                    _extIncoming.Add(new ClientProtocolIDMap(client, pair.Key, (byte)i));

                    IBTExtension ext = FindIBTExtensionByProtocol(pair.Key);

                    if (ext != null)
                    {
                        ext.OnHandshake(client, buffer);
                    }
                }
            }
            else
            {
                String protocol = FindIBTProtocolByMessageID(msgId);
                IBTExtension ext = FindIBTExtensionByProtocol(protocol);

                if (ext != null)
                {
                    ext.OnExtendedMessage(client, buffer);
                }
            }
        }
    }
}