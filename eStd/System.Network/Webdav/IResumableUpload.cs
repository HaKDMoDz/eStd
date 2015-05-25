﻿using System.IO;

namespace Creek.Net.Webdav {
    namespace Client {
        public interface IResumableUpload {
            void CancelUpload();
            void CancelUpload(string lockToken);
            long GetBytesUploaded();
            Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize);
            Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize, string contentType);
            Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize, string contentType, string lockToken);
        }

        public class WebDavResumableUpload {
            private long _bytesUploaded = 0;

            public void CancelUpload() {

            }

            public void CancelUpload(string lockToken) {
                
            }

            public long GetBytesUploaded() {
                return this._bytesUploaded;
            }

            public Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize) {
                return new MemoryStream();
            }

            public Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize, string contentType)
            {
                return new MemoryStream();
            }

            public Stream GetWriteStream(long startIndex, long contentLength, long resourceTotalSize, string contentType, string lockToken)
            {
                return new MemoryStream();
            }
        }
    }
}
