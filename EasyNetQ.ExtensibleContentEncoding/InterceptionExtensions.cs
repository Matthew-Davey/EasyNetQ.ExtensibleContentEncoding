namespace EasyNetQ.ExtensibleContentEncoding {
    using System;
    using global::EasyNetQ.Interception;

    /// <summary>
    /// Defines extension methods for the <see cref="IInterceptorRegistrator"/> interface.
    /// </summary>
    public static class InterceptionExtensions {
        /// <summary>
        /// Enable extensible content encoding and decoding based on the content_encoding message attribute.
        /// </summary>
        /// <param name="interceptorRegistrator">The extension instance.</param>
        /// <param name="configureCodecs">An optional callback which can be used to configure the codec map, adding
        /// new custom codecs or replacing existing ones.</param>
        /// <returns>The extension instance.</returns>
        public static IInterceptorRegistrator EnableExtensibleContentEncoding(this IInterceptorRegistrator interceptorRegistrator, Action<ICodecMap> configureCodecs = null) {
            if (interceptorRegistrator == null)
                throw new ArgumentNullException(nameof(interceptorRegistrator));

            interceptorRegistrator.Add(new ExtensibleContentEncodingInterceptor(configureCodecs));

            return interceptorRegistrator;
        }
    }
}
