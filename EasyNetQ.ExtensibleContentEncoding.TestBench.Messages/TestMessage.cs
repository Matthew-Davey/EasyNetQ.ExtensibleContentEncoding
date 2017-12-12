namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Messages {
    using System;
    using System.Runtime.Serialization;
    using EasyNetQ.MetaData.Abstractions;

    public class TestMessage {
        public String Content { get; set; }

        [MessageProperty(Property.ContentEncoding), IgnoreDataMember]
        public String ContentEncoding { get; set; }
    }
}
