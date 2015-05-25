namespace Creek.IO.Internal.Binary
{
    internal class BinaryRuntime
    {
        private static TypeBinaryDict data = new TypeBinaryDict();

        public static void Add(Creek.IO.Internal.Binary.Binary b)
        {
            data.AddC(b.GetType(), b);
        }
        public static void Add<T>() where T : Creek.IO.Internal.Binary.Binary, new()
        {
            Add(new T());
        }

        public static TypeBinaryDict Gets()
        {
            return data;
        }

    }
}
