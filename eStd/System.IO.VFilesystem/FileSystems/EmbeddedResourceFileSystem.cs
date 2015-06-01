using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System.IO.VFilesystem.FileSystems
{
    public class EmbeddedResourceFileSystem : SharpFileSystem.IFileSystem
    {
        public Assembly Assembly { get; private set; }
        public EmbeddedResourceFileSystem(Assembly assembly)
        {
            Assembly = assembly;
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            if (!path.IsRoot)
                throw new DirectoryNotFoundException();
            return Assembly.GetManifestResourceNames().Select(name => SharpFileSystem.FileSystemPath.Root.AppendFile(name)).ToArray();
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return path.IsRoot || !path.IsDirectory && Assembly.GetManifestResourceNames().Contains(path.EntityName);
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            if (access == FileAccess.Write)
                throw new NotSupportedException();
            if (path.IsDirectory || path.ParentPath != SharpFileSystem.FileSystemPath.Root)
                throw new FileNotFoundException();
            return Assembly.GetManifestResourceStream(path.EntityName);
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}
