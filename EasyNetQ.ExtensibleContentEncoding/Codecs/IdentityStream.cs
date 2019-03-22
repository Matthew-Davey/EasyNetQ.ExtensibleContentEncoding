﻿using System;
using System.IO;

namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    class IdentityStream : Stream {
        readonly Stream _inner;

        public IdentityStream(Stream inner) {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override bool CanRead => _inner.CanRead;
        public override bool CanSeek => _inner.CanSeek;
        public override bool CanWrite => _inner.CanWrite;
        public override long Length => _inner.Length;

        public override long Position {
            get => _inner.Position;
            set => _inner.Position = value;
        }

        public override void Flush() {
            _inner.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return _inner.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) {
            return _inner.Seek(offset, origin);
        }

        public override void SetLength(long value) {
            _inner.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count) {
            _inner.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }
    }
}
