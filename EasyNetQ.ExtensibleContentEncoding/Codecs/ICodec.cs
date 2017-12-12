namespace EasyNetQ.ExtensibleContentEncoding.Codecs {
    using System.IO;

    /// <summary>
    /// Defines the interface for an object which can encode and decode message payloads, and can be assembled
    /// into a pipeline.
    /// </summary>
    public interface ICodec {
        /// <summary>
        /// Adds a new stage to the encoding pipeline.
        /// </summary>
        /// <param name="pipeline">A stream representing the existing encoding pipeline.</param>
        /// <returns>A new stream representing the new encoding pipeline with an additional stage.</returns>
        Stream AddEncodingStage(Stream pipeline);

        /// <summary>
        /// Adds a new stage to the decoding pipeline.
        /// </summary>
        /// <param name="pipeline">A stream representing the existing decoding pipeline.</param>
        /// <returns>A new stream representing the new decoding pipeline with an additional stage.</returns>
        Stream AddDecodingStage(Stream pipeline);
    }
}
