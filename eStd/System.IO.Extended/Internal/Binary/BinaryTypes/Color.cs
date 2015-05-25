using System.IO;

namespace Creek.IO.Internal.Binary.BinaryTypes
{
    class Color : Creek.IO.Internal.Binary.Binary
    {

        public Color()
        {
            OnRead = OnReads;
            OnWrite = OnWrites;
        }

        private void OnWrites(BinaryWriter bw, object value)
        {
            var c = (System.Drawing.Color)value; bw.Write(c.A);
            bw.Write(c.R); bw.Write(c.G); bw.Write(c.B); 
        }

        private object OnReads(BinaryReader br)
        {
            return System.Drawing.Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte(),
                                                 br.ReadByte());
        }
    }
}
