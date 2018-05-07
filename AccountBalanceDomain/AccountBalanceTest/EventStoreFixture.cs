using System;
using System.IO;
using System.Net;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Embedded;
using EventStore.Core;
using Microsoft.Extensions.Configuration;
using ReactiveDomain;
using ReactiveDomain.EventStore;
using ReactiveDomain.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime.Serialization.JsonNet;
using NodaTime;

namespace AccountBalanceTest
{

    public class EventStoreFixture : IDisposable
    {
        readonly StreamStoreRepository _repo;
        readonly ClusterVNode _node;

        public EventStoreFixture()
        {
            var inMemory = true;
            var hostIp = "127.0.0.1";
            var port = 1113;

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                //.AddJsonFile("AppSettings.json")
                ;

            builder.AddJsonFile("AppSettings.json");
            builder.AddEnvironmentVariables();
            var config = builder.Build();

            var appSettings = config.GetSection("appSettings");
            if (appSettings != null)
            {
                if (!bool.TryParse(appSettings["InMemoryEventStore"], out inMemory))
                    inMemory = true;

                if (!inMemory)
                {
                    hostIp = appSettings["EventStoreIp"];
                    if (!int.TryParse(appSettings["EventStorePort"], out port))
                        port = 1113;
                }
            }

            var conns = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"))
                .Build();

            IEventStoreConnection eventStoreConnection;
            if (inMemory)
            {
                _node = EmbeddedVNodeBuilder
                    .AsSingleNode()
                    .OnDefaultEndpoints()
                    .RunInMemory()
                    .DisableDnsDiscovery()
                    .DisableHTTPCaching()
                    //.DisableScavengeMerging()
                    .DoNotVerifyDbHashes()
                    .Build();

                _node.StartAndWaitUntilReady().Wait();

                eventStoreConnection = EmbeddedEventStoreConnection.Create(_node, conns);
            }
            else
            {
                eventStoreConnection =
                    EventStoreConnection.Create(conns, new IPEndPoint(IPAddress.Parse(hostIp), port));
            }

            eventStoreConnection.ConnectAsync().Wait();

            StreamStoreConnection = new EventStoreConnectionWrapper(eventStoreConnection);

            EventSerializer = new JsonMessageSerializer();
            StreamNameBuilder = new PrefixedCamelCaseStreamNameBuilder("cqrsTraining");

            _repo = new StreamStoreRepository(StreamNameBuilder, StreamStoreConnection, EventSerializer);
        }

        public void Dispose()
        {
            StreamStoreConnection.Close();
            StreamStoreConnection.Dispose();

            _node?.Stop();
        }

        public IRepository Repository => _repo;

        public IStreamStoreConnection StreamStoreConnection { get; }

        public IStreamNameBuilder StreamNameBuilder { get; }

        public IEventSerializer EventSerializer { get; }
    }




public class EventStoreFixture_old : IDisposable
    {
        readonly StreamStoreRepository _repo;
        readonly ClusterVNode _node;

        public EventStoreFixture_old()
        {
            _node = EmbeddedVNodeBuilder
                .AsSingleNode()
                .OnDefaultEndpoints()
                .RunInMemory()
                .DisableDnsDiscovery()
                .DisableHTTPCaching()
                //.DisableScavengeMerging()
                .DoNotVerifyDbHashes()
                .Build();

            _node.StartAndWaitUntilReady().Wait();

            var conns = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new EventStore.ClientAPI.SystemData.UserCredentials("admin", "changeit"))
                .Build();

            var eventStoreConnection = EmbeddedEventStoreConnection.Create(_node, conns);

            StreamStoreConnection = new EventStoreConnectionWrapper(eventStoreConnection);

            EventSerializer = new JsonMessageSerializer();
            StreamNameBuilder = new PrefixedCamelCaseStreamNameBuilder("masterdata");

            _repo = new StreamStoreRepository(StreamNameBuilder, StreamStoreConnection, EventSerializer);
        }

        public void Dispose()
        {
            StreamStoreConnection.Close();
            StreamStoreConnection.Dispose();

            _node.Stop();
        }

        public IRepository Repository => _repo;

        public IStreamStoreConnection StreamStoreConnection { get; }

        public IStreamNameBuilder StreamNameBuilder { get; }

        public IEventSerializer EventSerializer { get; }
    }
}
