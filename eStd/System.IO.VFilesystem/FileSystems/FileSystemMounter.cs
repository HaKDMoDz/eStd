using System.Collections.Generic;
using System.IO;
using System.IO.VFilesystem.Collections;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class FileSystemMounter : SharpFileSystem.IFileSystem
    {
        public ICollection<KeyValuePair<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem>> Mounts { get; private set; }

        public FileSystemMounter(IEnumerable<KeyValuePair<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem>> mounts)
        {
            Mounts = new SortedList<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem>(new InverseComparer<SharpFileSystem.FileSystemPath>(Comparer<SharpFileSystem.FileSystemPath>.Default));
            foreach (var mount in mounts)
                Mounts.Add(mount);
        }

        public FileSystemMounter(params KeyValuePair<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem>[] mounts) : this((IEnumerable<KeyValuePair<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem>>)mounts)
        {
        }

        protected KeyValuePair<SharpFileSystem.FileSystemPath, SharpFileSystem.IFileSystem> Get(SharpFileSystem.FileSystemPath path)
        {
            return Mounts.First(pair => pair.Key == path || pair.Key.IsParentOf(path));
        }

        public void Dispose()
        {
            foreach (var mount in Mounts.Select(p => p.Value))
                mount.Dispose();
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            var pair = Get(path);
            var entities = pair.Value.GetEntities(path.IsRoot ? path : path.RemoveParent(pair.Key));
            return new EnumerableCollection<SharpFileSystem.FileSystemPath>(entities.Select(p => pair.Key.AppendPath(p)), entities.Count);
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            var pair = Get(path);
            return pair.Value.Exists(path.RemoveParent(pair.Key));
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            var pair = Get(path);
            return pair.Value.CreateFile(path.RemoveParent(pair.Key));
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            var pair = Get(path);
            return pair.Value.OpenFile(path.RemoveParent(pair.Key), access);
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            var pair = Get(path);
            pair.Value.CreateDirectory(path.RemoveParent(pair.Key));
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            var pair = Get(path);
            pair.Value.Delete(path.RemoveParent(pair.Key));
        }
    }
}
