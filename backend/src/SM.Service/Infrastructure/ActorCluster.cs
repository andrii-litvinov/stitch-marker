using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using SM.Service.Patterns;

namespace SM.Service.Infrastructure
{
    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IServiceProvider serviceProvider;

        public ActorCluster(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            T CreateInstance<T>() => ActivatorUtilities.CreateInstance<T>(serviceProvider);

            Remote.RegisterKnownKind(ActorKind.Pattern, Actor.FromProducer(CreateInstance<PatternActor>));
            Remote.RegisterKnownKind(ActorKind.PatternsPerOwner, Actor.FromProducer(CreateInstance<PatternsPerOwnerActor>));

            var provider = new ConsulProvider(new ConsulProviderOptions(), c => c.Address = new Uri(configuration["CONSUL_URL"]));
            Cluster.Start("PatternCluster", "127.0.0.1", 12001, provider);

            await Cluster.GetAsync(ActorKind.PatternsPerOwner, ActorKind.PatternsPerOwner, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
