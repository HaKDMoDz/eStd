// <copyright file="ByteArrayExtensions.cs" company="Edge Extensions Project">
// Copyright (c) 2009 All Rights Reserved
// </copyright>
// <author>Kevin Nessland</author>
// <email>kevinnessland@gmail.com</email>
// <date>2009-07-08</date>
// <summary>Contains byte array extension methods.</summary>

using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Creek.Extensions
{
    /// <summary>
    /// Contains various byte array related extensions.
    /// </summary>
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Deserializes a binary array.
        /// </summary>
        /// <param name="bytes">Array to deserialize.</param>
        /// <returns>Deserialized object.</returns>
        public static object BinaryDeserialize(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;

            return bf1.Deserialize(ms);
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Resulting string.</returns>
        public static string ByteArrayToString(this byte[] bytes)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(bytes);
        }

        /// <summary>
        /// Compresses a byte array using gzip compression.
        /// </summary>
        /// <param name="bytes">Byte array to compress.</param>
        /// <returns>A compressed byte array.</returns>
        public static byte[] Compress(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(bytes, 0, bytes.Length);
            zip.Close();
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzipBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzipBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, gzipBuffer, 0, 4);

            return gzipBuffer;
        }

        /// <summary>
        /// Decompresses a byte array using gzip compression.
        /// </summary>
        /// <param name="bytes">Byte array to decompress.</param>
        /// <returns>A decompressed byte array.</returns>
        public static byte[] Decompress(this byte[] bytes)
        {
            MemoryStream ms = new MemoryStream();
            int msgLength = BitConverter.ToInt32(bytes, 0);
            ms.Write(bytes, 4, bytes.Length - 4);

            byte[] buffer = new byte[msgLength];

            ms.Position = 0;
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
            zip.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        /// <summary>
        /// Converts a byte array to an image.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Converted image.</returns>
        public static Image ToImage(this byte[] bytes)
        {
            if (bytes != null)
            {
                MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
                ms.Write(bytes, 0, bytes.Length);

                return Image.FromStream(ms, true);
            }

            return null;
        }

        /// <summary>
        /// Trims the length of a byte array.
        /// </summary>
        /// <param name="bytes">Byte array to trim.</param>
        /// <param name="length">Number of bytes to trim.</param>
        /// <returns>The resulting byte array.</returns>
        public static byte[] Trim(this byte[] bytes, int length)
        {
            byte[] result = new byte[bytes.Length - length];

            Array.Copy(bytes, result, bytes.Length - length);

            return result;
        }

        /// <summary>
        /// Writes a byte array to a file.
        /// </summary>
        /// <param name="bytes">Byte array to write to the file.</param>
        /// <param name="fileName">Full path and name of the file to create.</param>
        /// <param name="fileMode">File mode.</param>
        /// <returns>Success indicator.</returns>
        public static bool ToFile(this byte[] bytes, string fileName, FileMode fileMode)
        {
            bool returnValue = true;

            FileAccess fileAccess = FileAccess.ReadWrite;

            if (fileMode == FileMode.Append)
            {
                fileAccess = FileAccess.Write;
            }

            FileStream fs = new FileStream(fileName, fileMode, fileAccess);

            BinaryWriter bw = new BinaryWriter(fs);

            try
            {
                bw.Write(bytes);
            }
            catch (Exception)
            {
                returnValue = false;
            }
            finally
            {
                fs.Close();

                bw.Close();
            }

            return returnValue;
        }
    }
}
