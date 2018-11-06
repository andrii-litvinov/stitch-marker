using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;
using SM.Service.Command;
using SM.Service.Patterns.Xsd;

namespace SM.Service.Patterns
{
    public class PatternActor : IActor
    {
        private readonly Behavior behavior = new Behavior();
        private readonly IEventStore eventStore;
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());
        private Pattern pattern;
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
                    await persistence.PersistEventAsync(new StitchesMarked
                    {
                        SourceId = command.PatternId,
                        Stitches = {command.Stitches}
                    });
                    break;
                case UnmarkStitches command:
                    await persistence.PersistEventAsync(new StitchesUnmarked
                    {
                        SourceId = command.PatternId,
                        Stitches = {command.Stitches}
                    });
                    break;
                case MarkBackstitches command:
                    await persistence.PersistEventAsync(new BackstitchesMarked
                    {
                        SourceId = command.PatternId,
                        Backstitches = {command.Backstitches}
                    });
                    break;
                case UnmarkBackstitches command:
                    await persistence.PersistEventAsync(new BackstitchesUnmarked
                    {
                        SourceId = command.PatternId,
                        Backstitches = {command.Backstitches}
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
                case BackstitchesMarked command:
                    MarkBackstitches(command.Backstitches, true);
                    break;
                case BackstitchesUnmarked command:
                    MarkBackstitches(command.Backstitches, false);
                    break;
                case StitchesMarked command:
                    MarkStitches(command.Stitches, true);
                    break;
                case StitchesUnmarked command:
                    MarkStitches(command.Stitches, false);
                    break;
            }
        }

        private void MarkStitches(IEnumerable<StitchCoordinates> stitches, bool marked)
        {
            foreach (var stitch in stitches)
            {
                var patternStitch = pattern.Stitches
                    .SingleOrDefault(item => item.X == stitch.X && item.Y == stitch.Y);
                if (patternStitch != null) patternStitch.Marked = marked;
            }
        }

        private void MarkBackstitches(IEnumerable<BackstitchCoordinates> backstitches, bool marked)
        {
            foreach (var backstitch in backstitches)
            {
                var patternBackstitch = pattern.Backstitches
                    .SingleOrDefault(item =>
                        item.X1 == backstitch.X1 &&
                        item.Y1 == backstitch.Y1 &&
                        item.X2 == backstitch.X2 &&
                        item.Y2 == backstitch.Y2);
                if (patternBackstitch != null) patternBackstitch.Marked = marked;
            }
        }
    }
}
