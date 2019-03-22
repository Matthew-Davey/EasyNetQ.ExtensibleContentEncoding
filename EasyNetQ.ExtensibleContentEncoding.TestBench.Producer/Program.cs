using System;
using EasyNetQ.ExtensibleContentEncoding.TestBench.Messages;
using EasyNetQ.Interception;
using EasyNetQ.Logging;
using EasyNetQ.MetaData;

namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Producer {
    class Program {
        static void Main() {
            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => {
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
