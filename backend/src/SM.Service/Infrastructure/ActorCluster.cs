using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Persistence;
using Proto.Remote;
using SM.Service.EventReader;
using SM.Service.Extensions;
using SM.Service.Infrastructure.EventStore;
using SM.Service.Patterns;
using SM.Service.PatternsManager;
using SM.Service.UserPatterns;

namespace SM.Service.Infrastructure
{
    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IEventStore eventStore;
        private readonly ISubscriptionEventStoreConnection subscriptionEventStoreConnection;

        public ActorCluster(IConfiguration configuration, IEventStore eventStore, ISubscriptionEventStoreConnection subscriptionEventStoreConnection)
        {
            this.configuration = configuration;
            this.eventStore = eventStore;
            this.subscriptionEventStoreConnection = subscriptionEventStoreConnection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Register all known actors in a generic way 
            var props = Actor.FromProducer(() => new PatternActor(eventStore));
            Remote.RegisterKnownKind("pattern", props);

            props = Actor.FromProducer(() => new EventReaderActor(subscriptionEventStoreConnection));
            Remote.RegisterKnownKind("eventReader", props);

            var provider = new ConsulProvider(new ConsulProviderOptions(),
                configuration1 => configuration1.Address = new Uri(configuration["CONSUL_URL"]));
            Cluster.Start("PatternCluster", "127.0.0.1", 12001, provider);
            
            await Remote.SpawnAsync("127.0.0.1:12001", "eventReader", 10.Seconds());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
