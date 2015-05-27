using System;
using System.IO;

namespace System.Network.Server
{
    public class Filehandler
    {
        private string _contentPath;

        public Filehandler(string contentPath)
        {
            _contentPath = contentPath;
        }

        internal bool DoesFileExists(string directory)
        {
            return File.Exists(_contentPath+directory);
        }

        internal byte[] ReadFile(string path)
        {
            //return File.ReadAllBytes(path);
            if (ServerCache.Contains(_contentPath+path))
            {
                Console.WriteLine("cache hit");
                return ServerCache.Get(_contentPath+path);
            }
            else
            {
                byte[] content = File.ReadAllBytes(_contentPath+path);
                ServerCache.Insert(_contentPath+path, content);
                return content;
            }

        }
    }
}
