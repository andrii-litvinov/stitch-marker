using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;
using SM.Service.Command;

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
                    await persistence.PersistEventAsync(new StitchUpdated
                    {
                        SourceId = pattern.Id,
                        Stitches = {command.Stitches},
                        Marked = true
                    });
                    break;
                case UnmarkStitches command:
                    await persistence.PersistEventAsync(new StitchUpdated
                    {
                        SourceId = pattern.Id,
                        Stitches = {command.Stitches},
                        Marked = false
                    });
                    break;
                case MarkBackstitches command:
                    await persistence.PersistEventAsync(new BackstitchUpdated
                    {
                        SourceId = pattern.Id,
                        Backstitches = {command.Backstitches},
                        Marked = true
                    });
                    break;
                case UnmarkBackstitches command:
                    await persistence.PersistEventAsync(new BackstitchUpdated
                    {
                        SourceId = pattern.Id,
                        Backstitches = {command.Backstitches},
                        Marked = false
                    });
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
                case StitchUpdated updated:
                    UpdateStitch(updated.SourceId, updated.Stitches, updated.Marked);
                    break;
                case BackstitchUpdated updated:
                    UpdateBackstitch(updated.SourceId, updated.Backstitches, updated.Marked);
                    break;
            }
        }

        private void UpdateStitch(string patternId, RepeatedField<StitchCoordinates> stitches, bool marked)
        {
            foreach (var stitch in stitches)
            {
                var patternStitch = pattern.Stitches
                    .FirstOrDefault(item => item.X == stitch.X && item.Y == stitch.Y);
                if (patternStitch != null) patternStitch.Marked = marked;
            }
        }

        private void UpdateBackstitch(string patternId, RepeatedField<BackstitchCoordinates> backstitches, bool marked)
        {
            foreach (var backstitch in backstitches)
            {
                var patternBackstitch = pattern.Backstitches
                    .FirstOrDefault(item =>
                        item.X1 == backstitch.X1 &&
                        item.Y1 == backstitch.Y1 &&
                        item.X2 == backstitch.X2 &&
                        item.Y2 == backstitch.Y2);
                if (patternBackstitch != null) patternBackstitch.Marked = marked;
            }
        }
    }
}
