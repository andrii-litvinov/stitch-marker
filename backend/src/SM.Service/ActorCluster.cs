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
using SM.Core;
using SM.Service.Classes;

namespace SM.Service
{
    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IPatternReader patternReader;
        private readonly IEventStore eventStore;

        public ActorCluster(IConfiguration configuration, IPatternReader patternReader, IEventStore eventStore)
        {
            this.configuration = configuration;
            this.patternReader = patternReader;
            this.eventStore = eventStore;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // TODO: Register all known actors in a generic way 
            var props = Actor.FromProducer(() => new Pattern(patternReader, eventStore));
            Remote.RegisterKnownKind("pattern", props);
            
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
