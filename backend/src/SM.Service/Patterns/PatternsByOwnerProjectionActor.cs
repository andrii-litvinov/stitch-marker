using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using SM.Service.Extensions;
using SM.Service.Infrastructure;
using SM.Service.Messages;

namespace SM.Service.Patterns
{
    public class PatternsByOwnerProjectionActor : IActor
    {
        private readonly Behavior behavior = new Behavior();
        private readonly Dictionary<string, PID> childByOwner = new Dictionary<string, PID>();
        private readonly Dictionary<string, PID> childBySource = new Dictionary<string, PID>();
        private readonly IActorFactory factory;
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());

        public PatternsByOwnerProjectionActor(IActorFactory factory) => this.factory = factory;

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.Spawn<EventSubscriptionActor>(factory);
                    behavior.Become(CatchingUp);
                    break;
                case LiveProcessingStarted _:
                    behavior.Become(LiveProcessing);
                    break;
                case IEvent @event:
                    switch (@event)
                    {
                        case PatternCreated created:
                            var pid = context.SpawnPrefix<PatternsByOwnerActor>(factory);
                            childByOwner.TryAdd(created.OwnerId, pid);
                            childBySource.TryAdd(created.SourceId, pid);
                            break;
                        case PatternDeleted deleted:
                            childBySource.Remove(deleted.SourceId);
                            break;
                    }

                    childBySource[@event.SourceId].Tell(@event);
                    break;
                default:
                    await behavior.ReceiveAsync(context);
                    break;
            }
        }

        private static async Task CatchingUp(IContext context) =>
            context.Sender.Tell(new CatchingUp());

        private async Task LiveProcessing(IContext context)
        {
            switch (context.Message)
            {
                case GetPatternItems query:
                    senders.Set(query.RequestId, context.Sender, 30.Seconds());
                    childByOwner[query.OwnerId].Tell(query);
                    break;
                case PatternItems items:
                    senders.Get<PID>(items.RequestId)?.Tell(items);
                    break;
            }
        }
    }
}
