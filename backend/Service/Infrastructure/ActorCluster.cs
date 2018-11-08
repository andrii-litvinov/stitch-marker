﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using Service.Patterns;

namespace Service
{
    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IActorFactory factory;

        public ActorCluster(IConfiguration configuration, IActorFactory factory) => (this.configuration, this.factory) = (configuration, factory);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Remote.RegisterKnownKind(ActorKind.Pattern, Actor.FromProducer(factory.Create<PatternActor>));
            Remote.RegisterKnownKind(ActorKind.PatternsByOwnerProjection, Actor.FromProducer(factory.Create<PatternsByOwnerProjectionActor>));

            var provider = new ConsulProvider(new ConsulProviderOptions(), c => c.Address = new Uri(configuration["CONSUL_URL"]));
            Cluster.Start("PatternCluster", "127.0.0.1", 12001, provider);

            while (true)
            {
                var (_, status) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection, cancellationToken);
                if (status is ResponseStatusCode.OK || status is ResponseStatusCode.ProcessNameAlreadyExist) break;
                await Task.Delay(100, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }    
}
