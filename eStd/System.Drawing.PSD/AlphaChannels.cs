using System.Collections.Generic;
using System.IO;

namespace System.Drawing.Psd
{
	public class AlphaChannels : ImageResource
	{
        private List<String> _channelNames;
        public IEnumerable<String> ChannelNames { get { return _channelNames;  } }

		public AlphaChannels()
			: base((Int16)ResourceIDs.AlphaChannelNames)
		{
            _channelNames = new List<String>();
		}

		public AlphaChannels(ImageResource imageResource)
			: base(imageResource)
		{
            _channelNames = new List<String>();
			BinaryReverseReader reverseReader = imageResource.DataReader;
			// the names are pascal strings without padding!!!
			while ((reverseReader.BaseStream.Length - reverseReader.BaseStream.Position) > 0)
			{
				Byte stringLength = reverseReader.ReadByte();
				String s = new String(reverseReader.ReadChars(stringLength));

                if (s.Length > 0) _channelNames.Add(s);
			}
			reverseReader.Close();
		}

		protected override void StoreData()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryReverseWriter reverseWriter = new BinaryReverseWriter(memoryStream);

			foreach (String name in ChannelNames)
			{
				reverseWriter.Write((Byte)name.Length);
				reverseWriter.Write(name.ToCharArray());
			}

			reverseWriter.Close();
			memoryStream.Close();

			Data = memoryStream.ToArray();
		}
	}
}