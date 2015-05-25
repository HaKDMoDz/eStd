namespace Creek.IO.Internal.Binary.BinaryTypes
{
    class BigInteger : Creek.IO.Internal.Binary.Binary
    {

        public BigInteger()
        {
            OnRead = OnReads;
            OnWrite = OnWrites;
        }

        private void OnWrites(Writer bw, object value)
        {
            var bi = (Creek.IO.Internal.Binary.Types.BigInteger)value;
            bw.Write(bi.LongValue());
        }

        private object OnReads(Reader br)
        {
            return new Creek.IO.Internal.Binary.Types.BigInteger(br.Read<long>().To<long>());
        }
    }
}
