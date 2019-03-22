using System;
using System.Collections.Generic;
using EasyNetQ.ExtensibleContentEncoding.Codecs;

namespace EasyNetQ.ExtensibleContentEncoding {
    class DefaultCodecFactory : ICodecMap {
        readonly IDictionary<string, ICodec> _codecs;

        public DefaultCodecFactory() {
            _codecs = new Dictionary<string, ICodec> {
                { "identity", new IdentityCodec() },
                { "gzip",     new GZipCodec() },
                { "x-gzip",   new GZipCodec() },
                { "deflate",  new DeflateCodec() },
                { "br",       new BrotliCodec() }
            };
        }

        public ICodec this[string key] {
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
