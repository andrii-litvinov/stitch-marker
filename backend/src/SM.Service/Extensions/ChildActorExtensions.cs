using System.Linq;
using Proto;
using SM.Service.Infrastructure;

namespace SM.Service.Extensions
{
    public static class ChildActorExtensions
    {
        public static PID GetChild<T>(this IContext context) where T : IActor, new()
        {
            var name = typeof(T).Name;
            return context.Children.FirstOrDefault(pid => pid.Id.EndsWith(name)) ??
                   context.SpawnNamed(Actor.FromProducer(() => new T()), name);
        }

        public static PID GetChild<T>(this IContext context, string name) where T : IActor, new()
        {
            return context.Children.FirstOrDefault(pid => pid.Id.EndsWith(name)) ??
                   context.SpawnNamed(Actor.FromProducer(() => new T()), name);
        }

        public static PID SpawnPrefix<T>(this IContext context, IActorFactory factory) where T : IActor =>
            context.SpawnPrefix(Actor.FromProducer(() => factory.Create<T>()), typeof(T).Name);
    }
}
