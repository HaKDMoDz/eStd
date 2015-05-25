using System;

namespace Creek.IO.Internal.Binary
{
    internal class Binary
    {
        public Action<Writer, object> OnWrite;
        public Func<Reader, object> OnRead;

        public Binary(Action<Writer, object> write, Func<Reader, object> read)
        {
            OnWrite = write;
            OnRead = read;
        }
        public Binary()
        {
            
        }

    }
}