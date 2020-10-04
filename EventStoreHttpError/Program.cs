using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace EventStoreHttpError
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // run against
            // .\EventStore-OSS-Windows-2019-v20.6.1\EventStore.ClusterNode.exe --insecure
            // make sure http://localhost:2113 works
            var connectionString = "esdb://admin:changeit@localhost:2113/?TlsVerifyCert=false&Tls=false";

            // run against
            // .\EventStore-OSS-Windows-2019-v20.6.0\EventStore.ClusterNode.exe --dev
            // make sure https://localhost:2113 works
            //var connectionString = "esdb://admin:changeit@localhost:2113/?TlsVerifyCert=false";

            var settings = EventStoreClientSettings.Create(connectionString);
            var client = new EventStoreClient(settings);

            await client.SubscribeToAllAsync(EventAppeared);
            Console.WriteLine("Subscribed to all events.");

            var data = Encoding.UTF8.GetBytes("{}");
            var eventData = new EventData(Uuid.NewUuid(), "test-event", data);
            await client.AppendToStreamAsync("test-events", StreamState.Any, new[] {eventData});

            Console.WriteLine("Keypress to exit.");
            Console.ReadKey();
        }

        private static Task EventAppeared(StreamSubscription stream, ResolvedEvent resolvedEvent, CancellationToken token)
        {
            Console.WriteLine("Event appeared : " + resolvedEvent.OriginalPosition);

            return Task.CompletedTask;
        }
    }
}
