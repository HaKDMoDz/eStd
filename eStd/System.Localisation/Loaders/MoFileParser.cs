using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Creek.I18N.Gettext.Loaders
{
    /// <summary>
    /// MO file format parser.
    /// See http://www.gnu.org/software/gettext/manual/html_node/MO-Files.html
    /// </summary>
    public class MoFileParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoFileParser"/> class.
        /// </summary>
        public MoFileParser()
        {
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Current encoding for decoding all strings in given MO file.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Read and load all translation strings from given MO file stream.
        /// </summary>
        /// <remarks>
        ///	http://www.gnu.org/software/gettext/manual/html_node/MO-Files.html
        /// </remarks>
        /// <param name="stream">Stream that contain binary data in the MO file format</param>
        /// <returns>Raw translations</returns>
        public Dictionary<string, string[]> GetTranslations(Stream stream)
        {
            Trace.WriteLine("Trying to parse a MO file stream...", "Lib.Gettext");

            if (stream == null || stream.Length < 20)
            {
                throw new ArgumentException("Stream can not be null of less than 20 bytes long.");
            }

            var reader = new BinaryReader(stream);
            uint magicNumber = reader.ReadUInt32();

            if (magicNumber != 0x950412de)
            {
                throw new ArgumentException("Invalid stream: can not find MO file magic number.");
            }

            uint revision = reader.ReadUInt32();
            Trace.WriteLine(String.Format("MO File Revision: {0}.{1}.", revision >> 16, revision & 0xffff),
                            "Lib.Gettext");

            if ((revision >> 16) > 1)
            {
                throw new Exception(String.Format("Unsupported MO file major revision: {0}.", revision >> 16));
            }

            int stringCount = reader.ReadInt32();
            int originalTableOffset = reader.ReadInt32();
            int translationlTableOffset = reader.ReadInt32();

            // We don't support hash tables and system dependent segments.

            Trace.WriteLine(String.Format("MO File contains {0} strings.", stringCount), "Lib.Gettext");


            var originalTable = new StringOffsetTable[stringCount];
            var translationlTable = new StringOffsetTable[stringCount];

            Trace.WriteLine(String.Format("Trying to parse strings using encoding \"{0}\"...", Encoding), "Lib.Gettext");

            reader.BaseStream.Seek(originalTableOffset, SeekOrigin.Begin);
            for (int i = 0; i < stringCount; i++)
            {
                originalTable[i].Length = reader.ReadInt32();
                originalTable[i].Offset = reader.ReadInt32();
            }

            reader.BaseStream.Seek(translationlTableOffset, SeekOrigin.Begin);
            for (int i = 0; i < stringCount; i++)
            {
                translationlTable[i].Length = reader.ReadInt32();
                translationlTable[i].Offset = reader.ReadInt32();
            }


            var dict = new Dictionary<string, string[]>(stringCount);

            for (int i = 0; i < stringCount; i++)
            {
                string[] originalStrings = _ReadStrings(reader, originalTable[i].Offset, originalTable[i].Length);
                string[] translatedStrings = _ReadStrings(reader, translationlTable[i].Offset,
                                                          translationlTable[i].Length);

                dict.Add(originalStrings[0], translatedStrings);
            }

            Trace.WriteLine("String parsing completed.", "Lib.Gettext");

            return dict;
        }

        private string[] _ReadStrings(BinaryReader reader, int offset, int length)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] stringBytes = reader.ReadBytes(length);
            return Encoding.GetString(stringBytes).Split('\0');
        }

        #region Nested type: StringOffsetTable

        private struct StringOffsetTable
        {
            public int Length;
            public int Offset;
        }

        #endregion
    }
}