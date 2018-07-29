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
        private readonly Dictionary<string, string> namesByOwner = new Dictionary<string, string>();
        private readonly Dictionary<string, string> namesBySource = new Dictionary<string, string>();
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
                            var name = $"patternsByOwner-{Guid.NewGuid()}";
                            namesByOwner.TryAdd(created.OwnerId, name);
                            namesBySource.TryAdd(created.SourceId, name);
                            break;
                        case PatternDeleted deleted:
                            namesBySource.Remove(deleted.SourceId);
                            break;
                    }

                    context.GetChild<PatternsByOwnerActor>(namesBySource[@event.SourceId]).Tell(@event);
                    break;
                case GetPatternItems query:
                    senders.Set(query.RequestId, context.Sender, 30.Seconds());
                    context.GetChild<PatternsByOwnerActor>(namesByOwner[query.OwnerId]).Tell(query);
                    break;
                case PatternItems items:
                    senders.Get<PID>(namesBySource[items.RequestId])?.Tell(items);
                    break;
            }
        }
    }
}
