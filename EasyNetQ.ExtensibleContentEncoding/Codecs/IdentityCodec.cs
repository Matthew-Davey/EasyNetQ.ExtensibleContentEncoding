namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    using System;
    using System.IO;

    class IdentityCodec : ICodec {
        public Stream AddEncodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new IdentityStream(baseStream);
        }

        public Stream AddDecodingStage(Stream baseStream) {
            if (baseStream == null)
                throw new ArgumentNullException(nameof(baseStream));

            return new IdentityStream(baseStream);
        }
    }
}
