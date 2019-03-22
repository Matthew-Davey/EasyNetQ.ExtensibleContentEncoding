using System;
using System.IO;
using System.IO.Compression;
using BrotliSharpLib;

namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    class BrotliCodec : ICodec {
        public Stream AddEncodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new BrotliStream(baseStream, CompressionMode.Compress);
        }

        public Stream AddDecodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new BrotliStream(baseStream, CompressionMode.Decompress);
        }
    }
}
