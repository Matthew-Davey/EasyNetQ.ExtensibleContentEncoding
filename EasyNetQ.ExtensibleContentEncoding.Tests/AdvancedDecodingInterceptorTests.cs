namespace EasyNetQ.ExtensibleContentEncoding.Tests {
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using EasyNetQ.ExtensibleContentEncoding.Codecs;
    using Xunit;

    using global::EasyNetQ.Interception;

    public class AdvancedDecodingInterceptorTests {
        [Fact]
        public void CodecIdentity() {
            var properties = new MessageProperties {
                ContentEncoding = "identity",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor();
            var encoded = subject.OnProduce(input);

            Assert.Equal("test", Encoding.UTF8.GetString(encoded.Body));

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        [Fact]
        public void CodecGzip() {
            var properties = new MessageProperties {
                ContentEncoding = "gzip",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor();
            var encoded = subject.OnProduce(input);

            Assert.Equal(new Byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 0, 11, 43, 73, 45, 46, 1, 0, 12, 126, 127, 216, 4, 0, 0, 0 }, encoded.Body);

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        [Fact]
        public void CodecDeflate() {
            var properties = new MessageProperties {
                ContentEncoding = "deflate",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor();
            var encoded = subject.OnProduce(input);

            Assert.Equal(new Byte[] { 43, 73, 45, 46, 1, 0 }, encoded.Body);

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        [Fact]
        public void CodecBrotli() {
            var properties = new MessageProperties {
                ContentEncoding = "br",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor();
            var encoded = subject.OnProduce(input);

            Assert.Equal(new Byte[] { 139, 1, 128, 116, 101, 115, 116, 3 }, encoded.Body);

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        [Fact]
        public void MultipleCodecs() {
            var properties = new MessageProperties {
                ContentEncoding = "deflate, gzip",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor();
            var encoded = subject.OnProduce(input);

            Assert.Equal(new Byte[] { 147, 239, 230, 96, 0, 3, 110, 109, 79, 93, 61, 70, 6, 158, 186, 250, 27, 44, 64, 46, 0 }, encoded.Body);

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        [Fact]
        public void CustomCodec() {
            var properties = new MessageProperties {
                ContentEncoding = "base64",
                ContentTypePresent = true
            };

            var input = new RawMessage(properties, Encoding.UTF8.GetBytes("test"));
            var subject = new ExtensibleContentEncodingInterceptor(codecs => {
                codecs["base64"] = new Base64Codec();
            });
            var encoded = subject.OnProduce(input);

            Assert.Equal(Encoding.UTF8.GetString(encoded.Body), "dGVzdA==");

            var decoded = subject.OnConsume(encoded);

            Assert.Equal("test", Encoding.UTF8.GetString(decoded.Body));
        }

        class Base64Codec : ICodec {
            public Stream AddDecodingStage(Stream baseStream) {
                return new CryptoStream(baseStream, new FromBase64Transform(), CryptoStreamMode.Read);
            }

            public Stream AddEncodingStage(Stream baseStream) {
                return new CryptoStream(baseStream, new ToBase64Transform(), CryptoStreamMode.Write);
            }
        }
    }
}
