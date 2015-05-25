namespace Creek.IO
{
    using System;
    using System.IO;
    using System.Text;

    public class StringStream : Stream
    {
        private readonly string _string;

        private readonly Encoding _encoding;

        private readonly long _byteLength;

        private int _position;

        public StringStream(string str, Encoding encoding = null)
        {
            this._string = str ?? string.Empty;
            this._encoding = encoding ?? Encoding.UTF8;

            this._byteLength = this._encoding.GetByteCount(this._string);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return this._byteLength; }
        }

        public override long Position
        {
            get
            {
                return this._position;
            }

            set
            {
                if (value < 0 || value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                this._position = (int)value;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this._byteLength + offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
            }

            return this.Position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this._position < 0)
            {
                throw new InvalidOperationException();
            }

            var bytesRead = 0;
            var chars = new char[1];

            // Loop until the buffer is full or the string has no more chars
            while (bytesRead < count && this._position < this._string.Length)
            {
                // Get the current char to encode
                chars[0] = this._string[this._position];

                // Get the required byte count for current char
                var byteCount = this._encoding.GetByteCount(chars);

                // If adding current char to buffer will exceed its length, do not add it
                if (bytesRead + byteCount > count)
                {
                    return bytesRead;
                }

                // Add the bytes of current char to byte buffer at next index
                this._encoding.GetBytes(chars, 0, 1, buffer, offset + bytesRead);

                // Increment the string position and total bytes read so far
                this.Position++;
                bytesRead += byteCount;
            }

            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }
}