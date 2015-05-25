using System;
using System.IO;

namespace Creek.IO
{
    public class TempFile : IDisposable
    {
        private readonly string _tmpfile;
        public TempFile()
            : this(string.Empty)
        {
        }

        public TempFile(string extension)
        {
            _tmpfile = Path.GetTempFileName();
            if (!string.IsNullOrEmpty(extension))
            {
                string newTmpFile = _tmpfile + extension;

                // create tmp-File with new extension ...
                File.Create(newTmpFile);
                // delete old tmp-File
                File.Delete(_tmpfile);

                // use new tmp-File
                _tmpfile = newTmpFile;
            }
        }

        public string FullPath
        {
            get { return _tmpfile; }
        }

        private void IDisposable_Dispose()
        {
            if (!string.IsNullOrEmpty(_tmpfile) && File.Exists(_tmpfile))
            {
                File.Delete(_tmpfile);
            }
        }
        void IDisposable.Dispose()
        {
            IDisposable_Dispose();
        }
    }
}