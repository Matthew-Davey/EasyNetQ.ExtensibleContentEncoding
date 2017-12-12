namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    using System;
    using System.IO;
    using System.IO.Compression;

    class DeflateCodec : ICodec {
        public Stream AddEncodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new DeflateStream(baseStream, CompressionMode.Compress);
        }

        public Stream AddDecodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new DeflateStream(baseStream, CompressionMode.Decompress);
        }
    }
}
