using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EasyNetQ.ExtensibleContentEncoding.Codecs;
using EasyNetQ.Interception;
using Xunit;

namespace EasyNetQ.ExtensibleContentEncoding.Tests {
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

            Assert.Equal(new byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 0, 3, 43, 73, 45, 46, 1, 0, 12, 126, 127, 216, 4, 0, 0, 0 }, encoded.Body);

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

            Assert.Equal(new byte[] { 43, 73, 45, 46, 1, 0 }, encoded.Body);

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

            Assert.Equal(new byte[] { 139, 1, 128, 116, 101, 115, 116, 3 }, encoded.Body);

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

            Assert.Equal(new byte[] { 147, 239, 230, 96, 0, 3, 102, 109, 79, 93, 61, 70, 6, 158, 186, 250, 27, 44, 64, 46, 0 }, encoded.Body);

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
