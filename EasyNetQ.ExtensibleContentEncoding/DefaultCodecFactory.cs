namespace EasyNetQ.ExtensibleContentEncoding {
    using System;
    using System.Collections.Generic;
    using EasyNetQ.ExtensibleContentEncoding.Codecs;

    class DefaultCodecFactory : ICodecMap {
        readonly IDictionary<String, ICodec> _codecs;

        public DefaultCodecFactory() {
            _codecs = new Dictionary<String, ICodec> {
                { "identity", new IdentityCodec() },
                { "gzip",     new GZipCodec() },
                { "x-gzip",   new GZipCodec() },
                { "deflate",  new DeflateCodec() },
                { "br",       new BrotliCodec() },
            };
        }

        public ICodec this[String key] {
            get {
                try {
                    return _codecs[key];
                }
                catch (KeyNotFoundException) {
                    throw new UnknownEncodingException(key);
                }
            }
            set => _codecs[key] = value;
        }
    }
}
