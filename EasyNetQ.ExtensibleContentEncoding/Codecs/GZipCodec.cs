using System;
using System.IO;
using System.IO.Compression;

namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    class GZipCodec : ICodec {
        public Stream AddEncodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new GZipStream(baseStream, CompressionMode.Compress);
        }

        public Stream AddDecodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new GZipStream(baseStream, CompressionMode.Decompress);
        }
    }
}
