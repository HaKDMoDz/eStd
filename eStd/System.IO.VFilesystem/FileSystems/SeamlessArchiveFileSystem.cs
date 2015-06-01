using System;
using System.Collections.Generic;
using System.IO;
using System.IO.VFilesystem.FileSystems;
using System.Linq;

namespace System.IO.VFilesystem.FileSystems
{
    public abstract class SeamlessArchiveFileSystem : SharpFileSystem.IFileSystem
    {
        public SharpFileSystem.IFileSystem FileSystem { get; private set; }

        public static readonly char ArchiveDirectorySeparator = '#';

        private FileSystemUsage _rootUsage;
        private IDictionary<SharpFileSystem.File, FileSystemUsage> _usedArchives = new Dictionary<SharpFileSystem.File, FileSystemUsage>();

        public SeamlessArchiveFileSystem(SharpFileSystem.IFileSystem fileSystem)
        {
            FileSystem = fileSystem;
            _rootUsage = new FileSystemUsage()
            {
                Owner = this,
                FileSystem = FileSystem,
                ArchiveFile = null
            };
        }

        public void UnuseFileSystem(FileSystemReference reference)
        {
            // When root filesystem was used.
            if (reference.Usage.ArchiveFile == null)
                return;

            FileSystemUsage usage;
            if (!_usedArchives.TryGetValue(reference.Usage.ArchiveFile, out usage))
                throw new ArgumentException("The specified reference is not valid.");
            if (!usage.References.Remove(reference))
                throw new ArgumentException("The specified reference does not exist.");
            if (usage.References.Count == 0)
            {
                _usedArchives.Remove(usage.ArchiveFile);

                usage.FileSystem.Dispose();
            }
        }

        protected abstract bool IsArchiveFile(SharpFileSystem.IFileSystem fileSystem, SharpFileSystem.FileSystemPath path);

        private SharpFileSystem.FileSystemPath ArchiveFileToDirectory(SharpFileSystem.FileSystemPath path)
        {
            if (!path.IsFile)
                throw new ArgumentException("The specified path is not a file.");
            return path.ParentPath.AppendDirectory(path.EntityName + ArchiveDirectorySeparator);
        }

        private SharpFileSystem.FileSystemPath GetRelativePath(SharpFileSystem.FileSystemPath path)
        {
            string s = path.ToString();
            int sindex = s.LastIndexOf(ArchiveDirectorySeparator.ToString() + SharpFileSystem.FileSystemPath.DirectorySeparator);
            if (sindex < 0)
                return path;
            return SharpFileSystem.FileSystemPath.Parse(s.Substring(sindex + 1));
        }

        protected bool HasArchive(SharpFileSystem.FileSystemPath path)
        {
            return path.ToString().LastIndexOf(ArchiveDirectorySeparator.ToString() + SharpFileSystem.FileSystemPath.DirectorySeparator) >= 0;
        }

        protected bool TryGetArchivePath(SharpFileSystem.FileSystemPath path, out SharpFileSystem.FileSystemPath archivePath)
        {
            string p = path.ToString();
            int sindex = p.LastIndexOf(ArchiveDirectorySeparator.ToString() + SharpFileSystem.FileSystemPath.DirectorySeparator);
            if (sindex < 0)
            {
                archivePath = path;
                return false;
            }
            archivePath = SharpFileSystem.FileSystemPath.Parse(p.Substring(0, sindex));
            return true;
        }

        protected FileSystemReference Refer(SharpFileSystem.FileSystemPath path)
        {
            SharpFileSystem.FileSystemPath archivePath;
            if (TryGetArchivePath(path, out archivePath))
                return CreateArchiveReference(archivePath);
            return new FileSystemReference(_rootUsage);
        }

        private FileSystemReference CreateArchiveReference(SharpFileSystem.FileSystemPath archiveFile)
        {
            return CreateReference((SharpFileSystem.File)GetActualLocation(archiveFile));
        }

        private FileSystemReference CreateReference(SharpFileSystem.File file)
        {
            var usage = GetArchiveFs(file);
            var reference = new FileSystemReference(usage);
            usage.References.Add(reference);
            return reference;
        }

        private SharpFileSystem.FileSystemEntity GetActualLocation(SharpFileSystem.FileSystemPath path)
        {
            SharpFileSystem.FileSystemPath archivePath;
            if (!TryGetArchivePath(path, out archivePath))
                return SharpFileSystem.FileSystemEntity.Create(FileSystem, path);
            var archiveFile = (SharpFileSystem.File)GetActualLocation(archivePath);
            FileSystemUsage usage = GetArchiveFs(archiveFile);
            return SharpFileSystem.FileSystemEntity.Create(usage.FileSystem, GetRelativePath(path));
        }

        private FileSystemUsage GetArchiveFs(SharpFileSystem.File archiveFile)
        {
            FileSystemUsage usage;
            if (_usedArchives.TryGetValue(archiveFile, out usage))
            {
                //System.Diagnostics.Debug.WriteLine("Open archives: " + _usedArchives.Count);
                return usage;
            }

            SharpFileSystem.IFileSystem archiveFs = CreateArchiveFileSystem(archiveFile);
            usage = new FileSystemUsage
            {
                Owner = this,
                FileSystem = archiveFs,
                ArchiveFile = archiveFile
            };
            _usedArchives[archiveFile] = usage;
            //System.Diagnostics.Debug.WriteLine("Open archives: " + _usedArchives.Count);
            return usage;
        }

        protected abstract SharpFileSystem.IFileSystem CreateArchiveFileSystem(SharpFileSystem.File archiveFile);

        public ICollection<SharpFileSystem.FileSystemPath> GetEntities(SharpFileSystem.FileSystemPath path)
        {
            using (var r = Refer(path))
            {
                var fileSystem = r.FileSystem;

                SharpFileSystem.FileSystemPath parentPath;
                if (TryGetArchivePath(path, out parentPath))
                    parentPath = ArchiveFileToDirectory(parentPath);
                else
                    parentPath = SharpFileSystem.FileSystemPath.Root;
                var entities = new LinkedList<SharpFileSystem.FileSystemPath>();
                foreach (var ep in fileSystem.GetEntities(GetRelativePath(path)))
                {
                    var newep = parentPath.AppendPath(ep.ToString().Substring(1));
                    entities.AddLast(newep);
                    if (IsArchiveFile(fileSystem, newep))
                        entities.AddLast(newep.ParentPath.AppendDirectory(newep.EntityName + ArchiveDirectorySeparator));
                }
                return entities;
            }
        }

        public bool Exists(SharpFileSystem.FileSystemPath path)
        {
            using (var r = Refer(path))
            {
                var fileSystem = r.FileSystem;
                return fileSystem.Exists(GetRelativePath(path));
            }
        }

        public Stream OpenFile(SharpFileSystem.FileSystemPath path, FileAccess access)
        {
            var r = Refer(path);
            var s = r.FileSystem.OpenFile(GetRelativePath(path), access);
            return new SafeReferenceStream(s, r);
        }

        #region Not implemented
        
        public System.IO.Stream CreateFile(SharpFileSystem.FileSystemPath path)
        {
            var r = Refer(path);
            var s = r.FileSystem.CreateFile(GetRelativePath(path));
            return new SafeReferenceStream(s, r);
        }
        
        public void CreateDirectory(SharpFileSystem.FileSystemPath path)
        {
            using (var r = Refer(path))
            {
                r.FileSystem.CreateDirectory(GetRelativePath(path));
            }
        }
        
        public void Delete(SharpFileSystem.FileSystemPath path)
        {
            using (var r = Refer(path))
            {
                r.FileSystem.Delete(GetRelativePath(path));
            }
        }
        
        #endregion
        
        public void Dispose()
        {
            foreach (var reference in _usedArchives.Values.SelectMany(usage => usage.References).ToArray())
                UnuseFileSystem(reference);
            FileSystem.Dispose();
        }
        
        public class DummyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }
        
        public class FileSystemReference : IDisposable
        {
            public FileSystemUsage Usage { get; set; }
            
            public SharpFileSystem.IFileSystem FileSystem
            {
                get
                {
                    return Usage.FileSystem;
                }
            }
            
            public FileSystemReference(FileSystemUsage usage)
            {
                Usage = usage;
            }
            
            public void Dispose()
            {
                Usage.Owner.UnuseFileSystem(this);
            }
        }
        
        public class FileSystemUsage
        {
            public SeamlessArchiveFileSystem Owner { get; set; }
            
            public SharpFileSystem.File ArchiveFile { get; set; }
            
            public SharpFileSystem.IFileSystem FileSystem { get; set; }
            
            public ICollection<FileSystemReference> References { get; set; }

            public FileSystemUsage()
            {
                References = new LinkedList<FileSystemReference>();
            }
        }

        public class SafeReferenceStream : Stream
        {
            private Stream _stream;
            private FileSystemReference _reference;

            public SafeReferenceStream(Stream stream, FileSystemReference reference)
            {
                _stream = stream;
                _reference = reference;
            }

            public override void Flush()
            {
                _stream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return _stream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                _stream.SetLength(value);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return _stream.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _stream.Write(buffer, offset, count);
            }

            public override bool CanRead
            {
                get { return _stream.CanRead; }
            }

            public override bool CanSeek
            {
                get { return _stream.CanSeek; }
            }

            public override bool CanWrite
            {
                get { return _stream.CanWrite; }
            }

            public override long Length
            {
                get { return _stream.Length; }
            }

            public override long Position
            {
                get { return _stream.Position; }
                set { _stream.Position = value; }
            }

            public override void Close()
            {
                _stream.Close();
                _reference.Dispose();
            }
        }
    }
}


