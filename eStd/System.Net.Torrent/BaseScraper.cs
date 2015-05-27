using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.Net.Torrent
{
    public abstract class BaseScraper
    {
        protected readonly Regex HashRegex = new Regex("^[a-f0-9]{40}$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        protected readonly Regex UDPRegex = new Regex("udp://([^:/]*)(?::([0-9]*))?(?:/)?", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        protected readonly Regex HTTPRegex = new Regex("(http://.*?/)announce?|scrape?([^/]*)$", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        protected readonly byte[] BaseCurrentConnectionId = { 0x00, 0x00, 0x04, 0x17, 0x27, 0x10, 0x19, 0x80 };
        protected readonly Random Random = new Random(DateTime.Now.Second);

        public Int32 Timeout { get; private set; }
        public String Tracker { get; private set; }
        public Int32 Port { get; private set; }

        protected BaseScraper(Int32 timeout)
        {
            Timeout = timeout;
        }

        public enum ScraperType
        {
            UDP,
            HTTP
        }

        protected void ValidateInput(String url, String[] hashes, ScraperType type)
        {
            if (hashes.Length < 1)
            {
                throw new ArgumentOutOfRangeException("hashes", hashes, "Must have at least one hash when calling scrape");
            }

            if (hashes.Length > 74)
            {
                throw new ArgumentOutOfRangeException("hashes", hashes, "Must have a maximum of 74 hashes when calling scrape");
            }

            foreach (String hash in hashes)
            {
                if (!HashRegex.IsMatch(hash))
                {
                    throw new ArgumentOutOfRangeException("hashes", hash, "Hash is not valid");
                }
            }

            if (type == ScraperType.UDP)
            {
                Match match = UDPRegex.Match(url);

                if (!match.Success)
                {
                    throw new ArgumentOutOfRangeException("url", url, "URL is not a valid UDP tracker address");
                }

                Tracker = match.Groups[1].Value;
                Port = match.Groups.Count == 3 ? Convert.ToInt32(match.Groups[2].Value) : 80;
            }
            else if (type == ScraperType.HTTP)
            {
                Match match = HTTPRegex.Match(url);

                if (!match.Success)
                {
                    throw new ArgumentOutOfRangeException("url", url, "URL is not a valid HTTP tracker address");
                }

                Tracker = match.Groups[0].Value;
            }
        }

        public class AnnounceInfo
        {
            public IEnumerable<EndPoint> Peers { get; set; }
            public Int32 WaitTime { get; set; }
            public Int32 Seeders { get; set; }
            public Int32 Leachers { get; set; }

            public AnnounceInfo(IEnumerable<EndPoint> peers, Int32 a, Int32 b, Int32 c)
            {
                Peers = peers;

                WaitTime = a;
                Seeders = b;
                Leachers = c;
            }
        }

        public class ScrapeInfo
        {
            public UInt32 Seeders { get; set; }
            public UInt32 Complete { get; set; }
            public UInt32 Leachers { get; set; }
            public UInt32 Downloaded { get; set; }
            public UInt32 Incomplete { get; set; }

            public ScrapeInfo(UInt32 a, UInt32 b, UInt32 c, ScraperType type)
            {
                if (type == ScraperType.HTTP)
                {
                    Complete = a;
                    Downloaded = b;
                    Incomplete = c;
                }
                else if (type == ScraperType.UDP)
                {
                    Seeders = a;
                    Complete = b;
                    Leachers = c;
                }
            }
        }
    }
}