using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EasyNetQ.ExtensibleContentEncoding.Codecs;
using EasyNetQ.Interception;

namespace EasyNetQ.ExtensibleContentEncoding
{
    /// <summary>
    /// Defines an implementation of <see cref="IProduceConsumeInterceptor"/> which encodes and decodes message content
    /// based on the value of the 'content_encoding' attribute.
    /// </summary>
    public class ExtensibleContentEncodingInterceptor : IProduceConsumeInterceptor {
        readonly ICodecMap _codecMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibleContentEncodingInterceptor"/> class.
        /// </summary>
        /// <param name="configureCodecs">An optional callback which can be used to configure the codec map, adding
        /// new custom codecs or replacing existing ones.</param>
        public ExtensibleContentEncodingInterceptor(Action<ICodecMap> configureCodecs = null) {
            _codecMap = new DefaultCodecFactory();

            if (configureCodecs != null)
                configureCodecs.Invoke(_codecMap);
        }

        /// <inheritdoc />
        public RawMessage OnProduce(RawMessage rawMessage) {
            var codecs = GetCodecSequence(rawMessage.Properties);

            using (var output = new MemoryStream()) {
                var encoderPipeline = codecs.Aggregate((Stream)output, (stream, codec) => codec.AddEncodingStage(stream));

                using (encoderPipeline) {
                    encoderPipeline.Write(rawMessage.Body, 0, rawMessage.Body.Length);
                }

                return new RawMessage(rawMessage.Properties, output.ToArray());
            }
        }

        /// <inheritdoc />
        public RawMessage OnConsume(RawMessage rawMessage) {
            var codecs = GetCodecSequence(rawMessage.Properties);

            using (var output = new MemoryStream())
            using (var payloadStream = new MemoryStream(rawMessage.Body)) {
                var decodePipeline = codecs.Aggregate((Stream)payloadStream, (stream, codec) => codec.AddDecodingStage(stream));

                using (decodePipeline) {
                    decodePipeline.CopyTo(output);
                    decodePipeline.Flush();

                    return new RawMessage(rawMessage.Properties, output.ToArray());
                }
            }
        }

        IEnumerable<ICodec> GetCodecSequence(MessageProperties properties)
        {
            if (properties.ContentEncodingPresent) {
                try {
                    return Regex.Matches(properties.ContentEncoding, @"(\w+)")
                        .OfType<Match>()
                        .Select(match => match.Value)
                        .Select(encoding => _codecMap[encoding]);
                }
                catch (ArgumentException) {
                    throw new Exception("Unable to parse content_encoding attribute: " + properties.ContentEncoding);
                }
            }

            return new [] { new IdentityCodec() };
        }
    }
}
