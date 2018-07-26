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
using SM.Service.Infrastructure.EventStore;
using SM.Service.OwnerPatterns;
using SM.Service.Patterns;

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

            var provider = new ConsulProvider(new ConsulProviderOptions(),
                configuration1 => configuration1.Address = new Uri(configuration["CONSUL_URL"]));
            Cluster.Start("PatternCluster", "127.0.0.1", 12001, provider);

            Remote.RegisterKnownKind("ownerPatterns", Actor.FromProducer(() => new OwnerPatternsActor()));
            await Cluster.GetAsync("ownerPatterns", "ownerPatterns", cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
