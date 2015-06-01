using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class ReadOnlyFileSystem : SharpFileSystem.IFileSystem
    {
        public SharpFileSystem.IFileSystem FileSystem { get; private set; }

        public ReadOnlyFileSystem(SharpFileSystem.IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }

        public void Dispose()
        {
            FileSystem.Dispose();
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            return FileSystem.GetEntities(path);
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return FileSystem.Exists(path);
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            if (access != FileAccess.Read)
                throw new InvalidOperationException("This is a read-only filesystem.");
            return FileSystem.OpenFile(path, access);
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            throw new InvalidOperationException("This is a read-only filesystem.");
        }
    }
}
