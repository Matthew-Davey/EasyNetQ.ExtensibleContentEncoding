using System;

namespace EasyNetQ.ExtensibleContentEncoding {
    /// <summary>
    /// Represents errors that occur when trying to encode or decode a message payload with an encoding that is not
    /// recognized or supported.
    /// </summary>
    public class UnknownEncodingException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownEncodingException"/> class.
        /// </summary>
        /// <param name="encoding">The encoding which is unknown or unsupported.</param>
        public UnknownEncodingException(string encoding)
            : base("Unknown encoding: " + encoding) {
        }
    }
}
