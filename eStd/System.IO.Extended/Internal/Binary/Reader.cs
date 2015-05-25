using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Creek.IO.Internal.Binary
{
    public class Reader : BinaryReader
    {
        public Reader(Stream input) : base(input)
        {
        }

        public Reader(Stream input, Encoding encoding) : base(input, encoding)
        {
        }

        [DebuggerStepThrough]
        public object Read<TT>(bool isArray = false)
        {
            var readers = Utils.InitTypes();
            readers.AddRange(BinaryRuntime.Gets());

            if (isArray)
            {
                var c = Read<int>().To<int>();
                var ret = new List<TT>();
                for (var i = 0; i < c; i++)
                {
                    ret.Add(Read<TT>().To<TT>());
                }
                return ret.ToArray();
            }
            else
            {
                if (!readers.ContainsKey(typeof(TT)))
                    throw new NotSupportedException(typeof(TT).Name + " is not supported");
                else
                    return (TT)readers[typeof(TT)].OnRead(this);
            }
        }
    }
}
