using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Torrent.Misc;

namespace System.Net.Torrent
{
    public class MagnetLink
    {
        public MagnetLink()
        {
            Trackers = new Collection<string>();
        }

        public String Name { get; set; }
        public byte[] Hash { get; set; }

        public String HashString
        {
            get { return Unpack.Hex(Hash); }
            set { Hash = Pack.Hex(value); }
        }

        public ICollection<String> Trackers { get; set; }

        public static MagnetLink Resolve(String magnetLink)
        {
            IEnumerable<KeyValuePair<String, String>> values = null;

            if (IsMagnetLink(magnetLink))
            {
                values = SplitURLIntoParts(magnetLink.Substring(8));
            }

            if (values == null) return null;

            MagnetLink magnet = new MagnetLink();

            foreach (KeyValuePair<string, string> pair in values)
            {
                if (pair.Key == "xt")
                {
                    if (!IsXTValidHash(pair.Value))
                    {
                        continue;
                    }

                    magnet.HashString = pair.Value.Substring(9);
                }

                if (pair.Key == "dn")
                {
                    magnet.Name = pair.Value;
                }

                if (pair.Key == "tr")
                {
                    magnet.Trackers.Add(pair.Value);
                }
            }

            return magnet;
        }

        public static Metadata ResolveToMetadata(String magnetLink)
        {
            return new Metadata(Resolve(magnetLink));
        }

        public static bool IsMagnetLink(String magnetLink)
        {
            return magnetLink.StartsWith("magnet:");
        }

        private static bool IsXTValidHash(String xt)
        {
            return xt.Length == 49 && xt.StartsWith("urn:btih:");
        }

        private static IEnumerable<KeyValuePair<String, String>> SplitURLIntoParts(String magnetLink)
        {
            String[] parts = magnetLink.Split('&');
            ICollection<KeyValuePair<String, String>> values = new Collection<KeyValuePair<string, string>>();

            foreach (String str in parts)
            {
                String[] kv = str.Split('=');
                values.Add(new KeyValuePair<string, string>(kv[0], Uri.UnescapeDataString(kv[1])));
            }

            return values;
        }
    }
}