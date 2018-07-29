using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using SM.Service.Extensions;
using SM.Service.Messages;

namespace SM.Service.Patterns
{
    public class PatternsByOwnerProjectionActor : IActor
    {
        private readonly Dictionary<string, PID> childByOwner = new Dictionary<string, PID>();
        private readonly Dictionary<string, PID> childBySource = new Dictionary<string, PID>();
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    // TODO: Spawn event subscriber actor and ignore all incoming messages until subscription switched to reading new messages.
                    break;
                case IEvent @event:
                    switch (@event)
                    {
                        case PatternCreated created:
                            var pid = context.SpawnPrefix<PatternsByOwnerActor>();
                            childByOwner.TryAdd(created.OwnerId, pid);
                            childBySource.TryAdd(created.SourceId, pid);
                            break;
                        case PatternDeleted deleted:
                            childBySource.Remove(deleted.SourceId);
                            break;
                    }

                    childBySource[@event.SourceId].Tell(@event);
                    break;
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
