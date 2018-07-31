using System;
using Microsoft.Extensions.DependencyInjection;
using Proto;

namespace SM.Service
{
    public interface IActorFactory
    {
        T Create<T>() where T : IActor;
    }

    public class ActorFactory : IActorFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ActorFactory(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        public T Create<T>() where T : IActor => ActivatorUtilities.CreateInstance<T>(serviceProvider);
    }
}
