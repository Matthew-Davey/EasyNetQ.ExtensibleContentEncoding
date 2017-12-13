# EasyNetQ.ExtensibleContentEncoding
An extensible EasyNetQ message intercepter which is content encoding aware.

[![Nuget Downloads](https://img.shields.io/nuget/dt/EasyNetQ.ExtensibleContentEncoding.svg)](https://www.nuget.org/packages/EasyNetQ.ExtensibleContentEncoding/) [![Nuget Version](https://img.shields.io/nuget/v/EasyNetQ.ExtensibleContentEncoding.svg)](https://www.nuget.org/packages/EasyNetQ.ExtensibleContentEncoding/)

ExtensibleContentEncoding serves two main purposes:
* Interoperabillity with other messaging middleware. EasyNetQ is a very opinionated framework and integrates flawlessly if used across the board. However it can become a little tricky when integrating with other messaging frameworks. ExtensibleContentEncoding allows you to consume messages from multiple different message sources which may be encoded or compressed differently.
* Zero downtime migration. Although EasyNetQ already supports gzip message compression, it's a global switch. Once enabled it expects *all* messages to be gzip encoded. This can cause problems if you want to migrate from uncompressed to compressed messages, or from one compression algorithm to another. With a global switch you would have to disable upstream producers, allow your consumer to drain the message queue, and then upgrade both producers and consumers to produce/consume compressed messages. With ExtensibleContentEncoding, you can make your downstream consumers ready for compressed messages before they're produced. Once the consumers are prepared, the producers can start producing compressed messages. You can do a phased rollout of the producers, without having to drain any queues. A queue can contain a mixture of both compressed and uncompressed messages during the migration phase.

ExtensibleContentEncoding supports the following encodings, modeled after the [HTTP content-encoding header specification](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Encoding).

Encoding | Algorithm
-------- | --------
identity\* | no compression, nor modification.
gzip     | A format using the [Lempel-Ziv coding](http://en.wikipedia.org/wiki/LZ77_and_LZ78#LZ77) (LZ77), with a 32-bit CRC.
deflate  | Using the [zlib](http://en.wikipedia.org/wiki/Zlib) structure (defined in [RFC 1950](http://tools.ietf.org/html/rfc1950)), with the [deflate](http://en.wikipedia.org/wiki/DEFLATE) compression algorithm (defined in [RFC 1951](http://tools.ietf.org/html/rfc1952)).
br       | A format using the [Brotli](https://en.wikipedia.org/wiki/Brotli) algorithm.

\* This is the default encoding, if none is specified

Additional encodings can be easily added, or existing encodings can be replaced with new implementations.

### Getting Started
Install the latest version of EasyNetQ.ExtensibleContentEncoding from NuGet - `Install-Package EasyNetQ.ExtensibleContentEncoding`.

```csharp
using EasyNetQ;
using EasyNetQ.Intercept;
using EasyNetQ.ExtensibleContentEncoding;

var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", serviceRegister => {
	serviceRegister.EnableInterception(intercept => {
		intercept.EnableExtensibleContentEncoding();
	});
});
```

If there are messages in the queue which are a mix of compressed or uncompressed, or a mixture of different encodings, each will now be decoded according to it's content_encoding attribute.

![extensiblecontentencoding01](https://user-images.githubusercontent.com/2029369/33917930-c30c2242-dfa8-11e7-8a14-5531acdb6ca2.png)

In this image, two messages with identical content are on the queue, but one has been compressed using the 'deflate' compression algorithm.

This could happen for example if you have 10 instances of your message producer running, each publishing uncompressed messages. You have made modifications to the producer service so that it publishes compressed messages using the deflate algorithm. But, during the rollout of the upgraded version, 3 instances have been upgraded whilsts 7 are still running the old version. During this phased rollout, both uncompressed and compressed messages are being produced. Downstream consumers need to be able to consume both in parallel.

### Custom encodings
It's straightforward to extend the library with new encodings. Take this example used in the unit tests:

```csharp
class Base64Codec : ICodec {
    public Stream AddDecodingStage(Stream baseStream) {
        return new CryptoStream(baseStream, new FromBase64Transform(), CryptoStreamMode.Read);
    }

    public Stream AddEncodingStage(Stream baseStream) {
        return new CryptoStream(baseStream, new ToBase64Transform(), CryptoStreamMode.Write);
    }
}

var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => {
    registrar.EnableInterception(interception => {
        interception.EnableExtensibleContentEncoding(codecs => {
            codecs["base64"] = new Base64Codec();
        });
    });
});
```

Now, any message where the `content_encoding` attribute is 'base64' will be decoded using our custom codec.

### Multiple encodings
As with the HTTP content-encoding header specification multiple encodings can be specified in order, separated by commas eg. 'deflate, gzip'.

### Outbound messages
ExtensibleContentEncoding will also encode any outbound messages according to the `content_encoding` attrbute set on the message properties. However EasyNetQ does not offer an easy way to set this attribute unless you publish your message using IAdvancedBus. Consider using the [EasyNetQ.MetaData](https://github.com/Matthew-Davey/EasyNetQ.MetaData) extension to set the content-encoding on a message by message basis, without having to use IAdvancedBus.
