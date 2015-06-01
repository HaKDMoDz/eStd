using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class FileSystemWrapper : SharpFileSystem.IFileSystem
    {
        public SharpFileSystem.IFileSystem Parent { get; private set; }

        public FileSystemWrapper(SharpFileSystem.IFileSystem parent)
        {
            Parent = parent;
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            return Parent.GetEntities(path);
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return Parent.Exists(path);
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            return Parent.CreateFile(path);
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            return Parent.OpenFile(path, access);
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            Parent.CreateDirectory(path);
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            Parent.Delete(path);
        }

        public void Dispose()
        {
            Parent.Dispose();
        }
    }
}
