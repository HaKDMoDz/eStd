using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Creek.IO.Internal.Binary;

namespace Creek.IO
{
    public class FileSystem
    {
        
        public interface IFs
        {
            string Name { get; set; }
        }
        [Serializable]
        public class File : IFs
        {
            public string Name { get; set; }
            public string Content { get; set; }
        }
        [Serializable]
        public class Folder
        {
            public string Name { get; set; }
            public List<IFs> Childs { get; set; }
        }

        public List<IFs> Childs = new List<IFs>();

        public void Save(Writer bw)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();

            bf.Serialize(ms, Childs);

            bw.Write<byte>(ms.ToArray(), true);
        }
        public void Load(Reader br)
        {
            var bf = new BinaryFormatter();
            var b = (byte[]) br.Read<byte>(true);
            Childs = (List<IFs>) bf.Deserialize(new MemoryStream(b));
        }

    }
}
