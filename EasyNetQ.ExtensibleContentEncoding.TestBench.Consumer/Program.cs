using System;
using System.Threading;
using EasyNetQ.ExtensibleContentEncoding.TestBench.Messages;
using EasyNetQ.Interception;
using EasyNetQ.Logging;
using EasyNetQ.MetaData;

namespace EasyNetQ.ExtensibleContentEncoding.TestBench.Consumer {
    class Program {
        static void Main() {
            LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

            var bus = RabbitHutch.CreateBus("host=localhost;username=guest;password=guest", registrar => {
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
