using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Extensions
{
    public static class BinaryExtensions
    {
        public static void Write(this BinaryWriter bw, DateTime dt)
        {
            bw.Write(dt.ToString());
        }

        public static DateTime ReadDateTime(this BinaryReader br)
        {
            return DateTime.Parse(br.ReadString());
        }

        public static void WriteStruct(this BinaryWriter bw, object strct)
        {
            byte[] buffer = new byte[Marshal.SizeOf(strct)];

            GCHandle h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(strct, h.AddrOfPinnedObject(), false);

            h.Free();

            bw.Write(buffer);
        }

        public static T ReadStruct<T>(this BinaryReader br)
            where T : struct
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            br.Read(buffer, 0, buffer.Count());

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            T result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return result;
        }
    }
}