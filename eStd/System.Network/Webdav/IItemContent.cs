using System.IO;

namespace Creek.Net.Webdav {
    namespace Client {
        public interface IItemContent {
            long ContentLength { get; }
            string ContentType { get; }

            void Download(string filename);
            void Upload(string filename);
            Stream GetReadStream();
            Stream GetWriteStream(long contentLength);
            Stream GetWriteStream(string contentType, long contentLength);
        }
    }
}
