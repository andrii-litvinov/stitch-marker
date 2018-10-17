using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;

namespace SM.Service.Patterns
{
    public class PatternActor : IActor
    {
        private readonly Behavior behavior = new Behavior();
        private readonly IEventStore eventStore;
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());
        private Service.Pattern pattern;
        private Persistence persistence;

        public PatternActor(IEventStore eventStore) => this.eventStore = eventStore;

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.SetReceiveTimeout(5.Minutes());
                    persistence = Persistence.WithEventSourcing(eventStore, context.Self.Id, ApplyEvent);
                    await persistence.RecoverStateAsync();
                    if (pattern == null) behavior.Become(New);
                    else behavior.Become(Created);
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                default:
                    await behavior.ReceiveAsync(context);
                    break;
            }
        }

        private async Task New(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    await persistence.PersistEventAsync(new PatternUploaded
                    {
                        SourceId = command.Id,
                        FileName = command.FileName,
                        Content = command.Content,
                        OwnerId = command.OwnerId
                    });
                    var parser = context.GetChild<XsdPatternActor>();
                    senders.Set(command.Id, context.Sender, 30.Seconds());
                    parser.Tell(command);
                    break;
                case PatternCreated @event:
                    await persistence.PersistEventAsync(@event);
                    senders.Get<PID>(pattern.Id)?.Tell(@event);
                    break;
            }
        }

        private async Task Created(IContext context)
        {
            switch (context.Message)
            {
                case GetPattern _:
                    context.Respond(pattern);
                    break;
                case GetThumbnail query:
                    query.Pattern = pattern;
                    var drawer = context.GetChild<PatternImageActor>();
                    senders.Set(query.Id, context.Sender, 30.Seconds());
                    drawer.Tell(query);
                    break;
                case Thumbnail thumbnail:
                    senders.Get<PID>(thumbnail.Id)?.Tell(thumbnail);
                    break;
                case DeletePattern command:
                    var patternDeleted = new PatternDeleted {SourceId = command.Id};
                    await persistence.PersistEventAsync(patternDeleted);
                    context.Sender.Tell(patternDeleted);
                    break;
                case GetPatternOwner _:
                    var patternOwner = new PatternOwner {OwnerId = pattern.OwnerId};
                    context.Sender.Tell(patternOwner);
                    break;
                case MarkStitches command:
                    await SetStitchMarked(command.Stitches, true);
                    break;
                case UnmarkStitches command:
                    await SetStitchMarked(command.Stitches, false);
                    break;
                case MarkBackstitches command:
                    await SetBackstitchMarked(command.Backstitches, true);
                    break;
                case UnmarkBackstitches command:
                    await SetBackstitchMarked(command.Backstitches, false);
                    break;
            }
        }

        private Task Deleted(IContext context) => Actor.Done;

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case PatternCreated e:
                    pattern = e.Pattern;
                    behavior.Become(Created);
                    break;
                case PatternDeleted _:
                    behavior.Become(Deleted);
                    break;
            }
        }
        
        private async Task SetBackstitchMarked(RepeatedField<BackstitchCoordinates> commandBackstitches, bool mark)
        {
            foreach (var backstitch in commandBackstitches)
                await persistence.PersistEventAsync(new BackstitchUpdated()
                {
                    SourceId = pattern.Id,
                    Backstitch = backstitch,
                    Marked = mark
                });
        }
        
        private async Task SetStitchMarked(RepeatedField<StitchCoordinates> commandStitches, bool mark)
        {
            foreach (var stitch in commandStitches)
                await persistence.PersistEventAsync(new StitchUpdated
                {
                    SourceId = pattern.Id,
                    Stitch = stitch,
                    Marked = mark
                });
        }
    }
}
