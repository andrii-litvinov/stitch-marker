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
        private readonly PatternAggregate pattern = new PatternAggregate();
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());
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
                    senders.Get<PID>(@event.SourceId)?.Tell(@event);
                    break;
            }
        }

        private async Task Created(IContext context)
        {
            switch (context.Message)
            {
                case GetPattern _:
                    context.Respond(pattern.GetPattern());
                    break;
                case GetThumbnail query:
                    query.Pattern = pattern.GetPattern();
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
                    context.Sender.Tell(pattern.GetPatternOwner());
                    break;
                case MarkStitches command:
                    var stitchesMarked = pattern.MarkStitches(command.Stitches);
                    await persistence.PersistEventAsync(stitchesMarked);
                    context.Sender.Tell(stitchesMarked);
                    break;
                case UnmarkStitches command:
                    var stitchesUnmarked = pattern.UnmarkStitches(command.Stitches);
                    await persistence.PersistEventAsync(stitchesUnmarked);
                    context.Sender.Tell(stitchesUnmarked);
                    break;
                case MarkBackstitches command:
                    var backstitchesMarked = pattern.MarkBackstitches(command.Backstitches);
                    await persistence.PersistEventAsync(backstitchesMarked);
                    context.Sender.Tell(backstitchesMarked);
                    break;
                case UnmarkBackstitches command:
                    var backstitchesUnmarked = pattern.UnmarkBackstitches(command.Backstitches);
                    await persistence.PersistEventAsync(backstitchesUnmarked);
                    context.Sender.Tell(backstitchesUnmarked);
                    break;
            }
        }

        private static Task Deleted(IContext context) => Actor.Done;

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case PatternCreated e:
                    pattern.Apply(e);
                    behavior.Become(Created);
                    break;
                case PatternDeleted _:
                    behavior.Become(Deleted);
                    break;
                case BackstitchesMarked e:
                    pattern.Apply(e);
                    break;
                case BackstitchesUnmarked e:
                    pattern.Apply(e);
                    break;
                case StitchesMarked e:
                    pattern.Apply(e);
                    break;
                case StitchesUnmarked e:
                    pattern.Apply(e);
                    break;
            }
        }
    }
}
