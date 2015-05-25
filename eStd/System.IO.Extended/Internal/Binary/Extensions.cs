namespace Creek.IO.Internal.Binary
{
    internal static class Extensions
    {

        public static T To<T>(this object o)
        {
            return (T) o;
        }

    }
}
