using System;
using System.Collections.Generic;

namespace System.Network.Server
{
    class ServerCache
    {
        public struct Content
        {
            internal byte[] ResponseContent;
            internal int RequestCount;
        };
        private static readonly object SyncRoot = new object();
        private static int _capacity = 15;
        private static Dictionary<string, Content> _cache = new Dictionary<string, Content>(StringComparer.OrdinalIgnoreCase) { };

        public static bool Insert(string url, byte[] body)
        {
            lock (SyncRoot)
            {
                if (IsFull())
                    CreateEmptySpace();

                var content = new Content {RequestCount = 0, ResponseContent = new byte[body.Length]};
                Buffer.BlockCopy(body, 0, content.ResponseContent, 0, body.Length);
                if (!_cache.ContainsKey(url))
                {
                    _cache.Add(url, content);
                    return false;
                }

                return true;
            }

        }

        public static bool IsFull()
        {
            return _cache.Count >= _capacity;
        }

        public static byte[] Get(string url)
        {
            if (_cache.ContainsKey(url))
            {
                Content content = _cache[url];
                content.RequestCount++;
                _cache[url] = content;
                return content.ResponseContent;
            }

            return null;
        }

        public static bool Contains(string url)
        {
            return _cache.ContainsKey(url);
        }

        private static void CreateEmptySpace()
        {
            var minRequestCount = Int32.MaxValue;
            var url = String.Empty;
            foreach (var entry in _cache)
            {
                Content content = entry.Value;
                if (content.RequestCount < minRequestCount)
                {
                    minRequestCount = content.RequestCount;
                    url = entry.Key;
                }
            }

            _cache.Remove(url);
        }

        public static int CacheCount()
        {
            return _cache.Count;
        }
    }
}
