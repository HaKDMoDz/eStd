using System;
using System.Collections.Generic;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class MergedFileSystem : SharpFileSystem.IFileSystem
    {
        public IEnumerable<SharpFileSystem.IFileSystem> FileSystems { get; private set; }

        public MergedFileSystem(IEnumerable<SharpFileSystem.IFileSystem> fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public MergedFileSystem(params SharpFileSystem.IFileSystem[] fileSystems)
        {
            FileSystems = fileSystems.ToArray();
        }

        public void Dispose()
        {
            foreach(var fs in FileSystems)
                fs.Dispose();
        }

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            var entities = new SortedList<SharpFileSystem.FileSystemPath, SharpFileSystem.FileSystemPath>();
            foreach (var fs in FileSystems.Where(fs => fs.Exists(path)))
            {
                foreach(var entity in fs.GetEntities(path))
                    if (!entities.ContainsKey(entity))
                        entities.Add(entity, entity);
            }
            return entities.Values;
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return FileSystems.Any(fs => fs.Exists(path));
        }

        public SharpFileSystem.IFileSystem GetFirst(SharpFileSystem.FileSystemPath path)
        {
            return FileSystems.FirstOrDefault(fs => fs.Exists(path));
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            var fs = GetFirst(path) ?? FileSystems.First();
            return fs.CreateFile(path);
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            var fs = GetFirst(path);
            if (fs == null)
                throw new FileNotFoundException();
            return fs.OpenFile(path, access);
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            if (Exists(path))
                throw new ArgumentException("The specified directory already exists.");
            var fs = GetFirst(path.ParentPath);
            if (fs == null)
                throw new ArgumentException("The directory-parent does not exist.");
            fs.CreateDirectory(path);
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            foreach(var fs in FileSystems.Where(fs => fs.Exists(path)))
                fs.Delete(path);
        }
    }
}
