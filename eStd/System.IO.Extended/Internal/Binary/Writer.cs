using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Creek.IO.Internal.Binary
{
    public class Writer : BinaryWriter
    {
        public Writer(Stream s) : base(s)
        {
        }

        [DebuggerStepThrough]
        public void Write<T>(object value, bool isArray = false)
        {
            var writers = Utils.InitTypes();
            writers.AddRange(BinaryRuntime.Gets());

            if (isArray)
            {
                var v = new List<T>((T[])value);
                Write<int>(v.Count);
                foreach (var vv in v)
                {
                    Write<T>(vv);
                }
            }
            else
            {
                if (!writers.ContainsKey(typeof(T)))
                    throw new NotSupportedException(typeof(T).Name + " is not supported");
                else
                    writers[typeof(T)].OnWrite(this, value);
            }
        }
    }
}
