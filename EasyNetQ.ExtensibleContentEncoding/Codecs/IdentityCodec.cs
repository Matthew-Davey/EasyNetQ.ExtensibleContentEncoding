using System;
using System.IO;

namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
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
