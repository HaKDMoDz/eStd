using System;
using System.Collections.Generic;

namespace Creek.IO.Internal.Binary
{
    internal class TypeBinaryDict : Dictionary<Type, Creek.IO.Internal.Binary.Binary>
    {

        public void AddC(Type t, Creek.IO.Internal.Binary.Binary b)
        {
            if(!ContainsKey(t))
                Add(t, b);
        }

        public void AddRange(TypeBinaryDict typeBinaryDict)
        {
            foreach (var t in typeBinaryDict)
            {
                AddC(t.Key, t.Value);
            }
        }
    }
}
