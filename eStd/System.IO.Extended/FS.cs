namespace Creek.IO
{
    using System.IO;

    public class FS : FileStream
    {
        public FS(string filename) : base(filename, FileMode.OpenOrCreate)
        {
            
        }
    }
}
