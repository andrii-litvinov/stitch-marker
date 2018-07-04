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
using SM.Service.Infrastructure.EventStore;
using SM.Service.Patterns;
using SM.Service.UserPatterns;

namespace SM.Service.Infrastructure
{
    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IEventStore eventStore;

        public ActorCluster(IConfiguration configuration, IEventStore eventStore)
        {
            this.configuration = configuration;
            this.eventStore = eventStore;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Register all known actors in a generic way 
            var props = Actor.FromProducer(() => new PatternActor(eventStore));
            Remote.RegisterKnownKind("pattern", props);

            props = Actor.FromProducer(() => new UserPatternsActor());
            Remote.RegisterKnownKind("user", props);

            props = Actor.FromProducer(() => new EventReaderActor(new StreamSubscriberConnection(configuration["EVENTSTORE_CONNECTION"])));
            var pid = Actor.SpawnNamed(props, "eventReader");

            var provider = new ConsulProvider(new ConsulProviderOptions(),
                configuration1 => configuration1.Address = new Uri(configuration["CONSUL_URL"]));
            Cluster.Start("PatternCluster", "127.0.0.1", 12001, provider);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
