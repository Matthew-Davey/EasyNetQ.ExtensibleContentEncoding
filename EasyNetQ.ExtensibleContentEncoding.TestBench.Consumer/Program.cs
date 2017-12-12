namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Consumer {
    using System;
    using System.Threading;
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

            using (var exitEvent = new ManualResetEventSlim())
            using (bus) {
                Console.CancelKeyPress += (_, args) => {
                    args.Cancel = true;
                    exitEvent.Set();
                };

                bus.Subscribe<TestMessage>("Consumer", message => {
                    Console.WriteLine("Message consumed. Content: " + message.Content);
                });

                Console.WriteLine("\n----------------------------------\n");

                exitEvent.Wait();
            }
        }
    }
}
