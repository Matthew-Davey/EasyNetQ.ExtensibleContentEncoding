namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    using System;
    using System.IO;

    class IdentityStream : Stream {
        readonly Stream _inner;

        public IdentityStream(Stream inner) {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override Boolean CanRead => _inner.CanRead;
        public override Boolean CanSeek => _inner.CanSeek;
        public override Boolean CanWrite => _inner.CanWrite;
        public override Int64 Length => _inner.Length;

        public override Int64 Position {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() {
            _inner.Flush();
        }

        public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count) {
            return _inner.Read(buffer, offset, count);
        }

        public override Int64 Seek(Int64 offset, SeekOrigin origin) {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(Int64 value) {
            _inner.SetLength(value);
        }

        public override void Write(Byte[] buffer, Int32 offset, Int32 count) {
            _inner.Write(buffer, offset, count);
        }

        protected override void Dispose(Boolean disposing) {
            base.Dispose(disposing);
        }
    }
}
