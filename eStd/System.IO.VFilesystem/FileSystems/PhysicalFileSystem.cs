using System;
using System.Collections.Generic;
using System.IO;
using System.IO.VFilesystem.Collections;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public class PhysicalFileSystem : SharpFileSystem.IFileSystem
    {
        #region Internals
        public string PhysicalRoot { get; private set; }

        public PhysicalFileSystem(string physicalRoot)
        {
            if (!Path.IsPathRooted(physicalRoot))
                physicalRoot = Path.GetFullPath(physicalRoot);
            if (physicalRoot[physicalRoot.Length - 1] != Path.DirectorySeparatorChar)
                physicalRoot = physicalRoot + Path.DirectorySeparatorChar;
            PhysicalRoot = physicalRoot;
        }

        public string GetPhysicalPath(SharpFileSystem.FileSystemPath path)
        {
            return Path.Combine(PhysicalRoot, path.ToString().Remove(0, 1).Replace(SharpFileSystem.FileSystemPath.DirectorySeparator, Path.DirectorySeparatorChar));
        }

        public SharpFileSystem.FileSystemPath GetVirtualFilePath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            string virtualPath = SharpFileSystem.FileSystemPath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, SharpFileSystem.FileSystemPath.DirectorySeparator);
            return SharpFileSystem.FileSystemPath.Parse(virtualPath);
        }

        public SharpFileSystem.FileSystemPath GetVirtualDirectoryPath(string physicalPath)
        {
            if (!physicalPath.StartsWith(PhysicalRoot, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException("The specified path is not member of the PhysicalRoot.", "physicalPath");
            string virtualPath = SharpFileSystem.FileSystemPath.DirectorySeparator + physicalPath.Remove(0, PhysicalRoot.Length).Replace(Path.DirectorySeparatorChar, SharpFileSystem.FileSystemPath.DirectorySeparator);
            if (virtualPath[virtualPath.Length - 1] != SharpFileSystem.FileSystemPath.DirectorySeparator)
                virtualPath += SharpFileSystem.FileSystemPath.DirectorySeparator;
            return SharpFileSystem.FileSystemPath.Parse(virtualPath);
        }

        #endregion

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            string physicalPath = GetPhysicalPath(path);
            string[] directories = System.IO.Directory.GetDirectories(physicalPath);
            string[] files = System.IO.Directory.GetFiles(physicalPath);
            var virtualDirectories =
                directories.Select(p => GetVirtualDirectoryPath(p));
            var virtualFiles =
                files.Select(p => GetVirtualFilePath(p));
            return new EnumerableCollection<SharpFileSystem.FileSystemPath>(virtualDirectories.Concat(virtualFiles), directories.Length + files.Length);
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            return path.IsFile ? System.IO.File.Exists(GetPhysicalPath(path)) : System.IO.Directory.Exists(GetPhysicalPath(path));
        }

        public Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return System.IO.File.Create(GetPhysicalPath(path));
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.", "path");
            return System.IO.File.Open(GetPhysicalPath(path), FileMode.Open, access);
        }

        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            if (!path.IsDirectory)
                throw new ArgumentException("The specified path is not a directory.", "path");
            System.IO.Directory.CreateDirectory(GetPhysicalPath(path));
        }

        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            if (path.IsFile)
                System.IO.File.Delete(GetPhysicalPath(path));
            else
                System.IO.Directory.Delete(GetPhysicalPath(path), true);
        }

        public void Dispose()
        {
        }
    }
}


