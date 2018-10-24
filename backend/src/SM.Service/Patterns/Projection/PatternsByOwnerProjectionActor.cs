using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Cluster;

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
                        case PatternUploaded uploaded:
                            CreateChild(context, uploaded.OwnerId, uploaded.SourceId);
                            break;
                        case PatternCreated created:
                            CreateChild(context, created.OwnerId, created.SourceId);
                            break;
                        case StitchUpdated updated:
                            await UpdateStitch(updated.SourceId, updated.Stitch, updated.Marked);
                            break;
                        case BackstitchUpdated updated:
                            await UpdateBackstitch(updated.SourceId, updated.Backstitch, updated.Marked);
                            break;
                    }

                    childBySource[@event.SourceId].Tell(@event);

                    if (@event is PatternDeleted deleted) childBySource.Remove(deleted.SourceId);

                    break;
                default:
                    await behavior.ReceiveAsync(context);
                    break;
            }
        }

        private static async Task CatchingUp(IContext context) =>
            context.Sender?.Tell(new CatchingUp());

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

        private void CreateChild(IContext context, string ownerId, string sourceId)
        {
            if (!childByOwner.TryGetValue(ownerId, out var pid))
            {
                pid = context.SpawnPrefix<PatternsByOwnerActor>(factory);
                childByOwner.Add(ownerId, pid);
            }

            childBySource.TryAdd(sourceId, pid);
        }

        private async Task UpdateStitch(string patternId, StitchCoordinates stitch, bool marked)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var patternItem = await pattern.RequestAsync<Service.Pattern>(new GetPattern {Id = patternId}, 10.Seconds());

            var patternStitch = patternItem.Stitches
                .FirstOrDefault(item => item.X == stitch.X && item.Y == stitch.Y);
            if (patternStitch != null) patternStitch.Marked = marked;
        }

        private async Task UpdateBackstitch(string patternId, BackstitchCoordinates backstitch, bool marked)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var patternItem = await pattern.RequestAsync<Service.Pattern>(new GetPattern {Id = patternId}, 10.Seconds());

            var patternBackstitch = patternItem.Backstitches
                .FirstOrDefault(item =>
                    item.X1 == backstitch.X1 &&
                    item.Y1 == backstitch.Y1 &&
                    item.X2 == backstitch.X2 &&
                    item.Y2 == backstitch.Y2);
            if (patternBackstitch != null) patternBackstitch.Marked = marked;
        }
    }
}
