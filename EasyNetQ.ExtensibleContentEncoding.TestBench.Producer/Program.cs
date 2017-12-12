namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Producer {
    using System;
    using EasyNetQ.ExtensibleContentEncoding;
    using EasyNetQ.ExtensibleContentEncoding.TestBench.Messages;
    using EasyNetQ.MetaData;

    using global::EasyNetQ;
    using global::EasyNetQ.Interception;
    using global::EasyNetQ.Loggers;

    class Program {
        static void Main() {
            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => {
                registrar.Register<IEasyNetQLogger>(_ => new ConsoleLogger());
                registrar.EnableMessageMetaDataBinding();

                registrar.EnableInterception(interception => {
                    interception.EnableExtensibleContentEncoding();
                });
            });

            using (bus) {
                while (true) {
                    Console.WriteLine("\n----------------------------------\n");

                    Console.Write("Enter message content > ");
                    var content = Console.ReadLine();

                    Console.Write("Enter content encoding eg. 'gzip' > ");
                    var encoding = Console.ReadLine();

                    bus.Publish(new TestMessage {
                        Content = content,
                        ContentEncoding = encoding
                    });
                }
            }
        }
    }
}
