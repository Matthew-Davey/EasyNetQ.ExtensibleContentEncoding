namespace EasyNetQ.ExtensibleContentEncoding {
    using System;
    using EasyNetQ.ExtensibleContentEncoding.Codecs;

    /// <summary>
    /// Defines mapping between content-encoding header values and <see cref="ICodec"/> objects.
    /// </summary>
    public interface ICodecMap {
        /// <summary>
        /// Gets or sets the <see cref="ICodec"/> object associated with the specified encoding value.
        /// </summary>
        /// <param name="key">The encoding value eg. 'gzip', 'deflate' etc.</param>
        /// <returns>A <see cref="ICodec"/> object which implements the specifed encoding.</returns>
        /// <exception cref="UnknownEncodingException">
        /// Thrown when trying to retrieve an encoding for which there is no <see cref="ICodec"/> associated.
        /// </exception>
        ICodec this[String key] { get; set; }
    }
}
