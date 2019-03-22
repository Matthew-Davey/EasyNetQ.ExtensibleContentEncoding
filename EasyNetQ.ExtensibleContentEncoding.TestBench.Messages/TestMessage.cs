using System;
using System.Runtime.Serialization;
using EasyNetQ.MetaData.Abstractions;

namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Messages {
    public class TestMessage {
        public string Content { get; set; }

        [MessageProperty(Property.ContentEncoding), IgnoreDataMember]
        public string ContentEncoding { get; set; }
    }
}
