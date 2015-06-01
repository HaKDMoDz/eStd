using System.Collections.Generic;
using System.IO;
using System.IO.VFilesystem.Collections;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class SubFileSystem : SharpFileSystem.IFileSystem
    {
        public SharpFileSystem.IFileSystem FileSystem { get; private set; }

        public SharpFileSystem.FileSystemPath Root { get; private set; }

        public SubFileSystem(SharpFileSystem.IFileSystem fileSystem, SharpFileSystem.FileSystemPath root)
        {
            FileSystem = fileSystem;
            Root = root;
        }

        protected SharpFileSystem.FileSystemPath AppendRoot(SharpFileSystem.FileSystemPath path)
        {
            return Root.AppendPath(path);
        }

        protected SharpFileSystem.FileSystemPath RemoveRoot(SharpFileSystem.FileSystemPath path)
        {
            return path.RemoveParent(Root);
        }

        public void Dispose()
        {
            FileSystem.Dispose();
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            var paths = FileSystem.GetEntities(AppendRoot(path));
            return new EnumerableCollection<SharpFileSystem.FileSystemPath>(paths.Select(p => RemoveRoot(p)), paths.Count);
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return FileSystem.Exists(AppendRoot(path));
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            return FileSystem.CreateFile(AppendRoot(path));
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            return FileSystem.OpenFile(AppendRoot(path), access);
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            FileSystem.CreateDirectory(AppendRoot(path));
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            FileSystem.Delete(AppendRoot(path));
        }
    }
}
