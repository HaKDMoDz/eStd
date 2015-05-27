using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Torrent.BEncode;
using System.Net.Torrent.Misc;
using System.Text;

namespace System.Net.Torrent
{
    public class HTTPTrackerClient : BaseScraper, ITrackerClient
    {
        public HTTPTrackerClient(int timeout) 
            : base(timeout)
        {

        }

        private static IEnumerable<IPEndPoint> GetPeers(byte[] peerData)
        {
            for (int i = 0; i < peerData.Length; i += 6)
            {
                long addr = Unpack.UInt32(peerData, i, Unpack.Endianness.Big);
                ushort port = Unpack.UInt16(peerData, i + 4, Unpack.Endianness.Big);

                yield return new IPEndPoint(addr, port);
            }
        }

        public IDictionary<string, AnnounceInfo> Announce(string url, string[] hashes, string peerId)
        {
            return hashes.ToDictionary(hash => hash, hash => Announce(url, hash, peerId));
        }

        public AnnounceInfo Announce(string url, string hash, string peerId)
        {
            return Announce(url, hash, peerId, 0, 0, 0, 2, 0, -1, 12345, 0);
        }

        public AnnounceInfo Announce(string url, string hash, string peerId, long bytesDownloaded, long bytesLeft, long bytesUploaded, 
            int eventTypeFilter, int ipAddress, int numWant, int listenPort, int extensions)
        {
            byte[] hashBytes = Pack.Hex(hash);
            byte[] peerIdBytes = Encoding.ASCII.GetBytes(peerId);

            String realUrl = url.Replace("scrape", "announce") + "?";

            String hashEncoded = "";
            foreach (byte b in hashBytes)
            {
                hashEncoded += String.Format("%{0:X2}", b);
            }

            String peerIdEncoded = "";
            foreach (byte b in peerIdBytes)
            {
                peerIdEncoded += String.Format("%{0:X2}", b);
            }

            realUrl += "info_hash=" + hashEncoded;
            realUrl += "&peer_id=" + peerIdEncoded;
            realUrl += "&port=" + listenPort;
            realUrl += "&uploaded=" + bytesUploaded;
            realUrl += "&downloaded=" + bytesDownloaded;
            realUrl += "&left=" + bytesLeft;
            realUrl += "&event=started";
            realUrl += "&compact=1";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            webRequest.Accept = "*/*";
            webRequest.UserAgent = "System.Net.Torrent";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            Stream stream = webResponse.GetResponseStream();

            if (stream == null) return null;

            BinaryReader binaryReader = new BinaryReader(stream);

            byte[] bytes = new byte[0];

            while (true)
            {
                try
                {
                    byte[] b = new byte[1];
                    b[0] = binaryReader.ReadByte();
                    bytes = bytes.Concat(b).ToArray();
                }
                catch (Exception)
                {
                    break;
                }
            }

            BDict decoded = (BDict)BencodingUtils.Decode(bytes);
            if (decoded.Count == 0)
            {
                return null;
            }

            if (!decoded.ContainsKey("peers"))
            {
                return null;
            }

            if (!(decoded["peers"] is BString))
            {
                throw new NotSupportedException("Dictionary based peers not supported");
            }

            Int32 waitTime = 0;
            Int32 seeders = 0;
            Int32 leachers = 0;

            if (decoded.ContainsKey("interval"))
            {
                waitTime = (BInt)decoded["interval"];
            }

            if (decoded.ContainsKey("complete"))
            {
                seeders = (BInt)decoded["complete"];
            }

            if (decoded.ContainsKey("incomplete"))
            {
                leachers = (BInt)decoded["incomplete"];
            }

            BString peerBinary = (BString)decoded["peers"];

            return new AnnounceInfo(GetPeers(peerBinary.ByteValue), waitTime, seeders, leachers);
        }

        public IDictionary<string, ScrapeInfo> Scrape(string url, string[] hashes)
        {
            Dictionary<String, ScrapeInfo> returnVal = new Dictionary<string, ScrapeInfo>();

            String realUrl = url.Replace("announce", "scrape") + "?";

            String hashEncoded = "";
            foreach (String hash in hashes)
            {
                byte[] hashBytes = Pack.Hex(hash);

                hashEncoded = hashBytes.Aggregate(hashEncoded, (current, b) => current + String.Format("%{0:X2}", b));

                realUrl += "info_hash=" + hashEncoded + "&";
            }

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(realUrl);
            webRequest.Accept = "*/*";
            webRequest.UserAgent = "System.Net.Torrent";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            Stream stream = webResponse.GetResponseStream();

            if (stream == null) return null;

            BinaryReader binaryReader = new BinaryReader(stream);

            byte[] bytes = new byte[0];
            
            while (true)
            {
                try
                {
                    byte[] b = new byte[1];
                    b[0] = binaryReader.ReadByte();
                    bytes = bytes.Concat(b).ToArray();
                }
                catch (Exception)
                {
                    break;
                }
            }

            BDict decoded = (BDict)BencodingUtils.Decode(bytes);
            if (decoded.Count == 0) return null;

            if (!decoded.ContainsKey("files")) return null;

            BDict bDecoded = (BDict)decoded["files"];

            foreach (String k in bDecoded.Keys)
            {
                BDict d = (BDict)bDecoded[k];

                if (d.ContainsKey("complete") && d.ContainsKey("downloaded") && d.ContainsKey("incomplete"))
                {
                    String rk = Unpack.Hex(BencodingUtils.ExtendedASCIIEncoding.GetBytes(k));
                    returnVal.Add(rk, new ScrapeInfo((uint)((BInt)d["complete"]).Value, (uint)((BInt)d["downloaded"]).Value, (uint)((BInt)d["incomplete"]).Value, ScraperType.HTTP));
                }
            }

            return returnVal;
        }
    }
}