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
        private PatternAggregate pattern;
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
                    behavior.Become(Started);
                    if (pattern != null)
                        behavior.Become(Active);
                    break;
                case ReceiveTimeout _:
                    context.Self.Stop();
                    break;
                default:
                    await behavior.ReceiveAsync(context);
                    break;
            }
        }

        private async Task Started(IContext context)
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

        private async Task Active(IContext context)
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
                case GetPatternOwner _:
                    context.Sender.Tell(pattern.GetPatternOwner());
                    break;
                case Thumbnail thumbnail:
                    senders.Get<PID>(thumbnail.Id)?.Tell(thumbnail);
                    break;
                case DeletePattern _:
                    await persistence.PersistEventAsync(pattern.Delete());
                    break;
                case MarkStitches command:
                    await PersistAndReply(context, pattern.MarkStitches(command.Stitches));
                    break;
                case UnmarkStitches command:
                    await PersistAndReply(context, pattern.UnmarkStitches(command.Stitches));
                    break;
                case MarkBackstitches command:
                    await PersistAndReply(context, pattern.MarkBackstitches(command.Backstitches));
                    break;
                case UnmarkBackstitches command:
                    await PersistAndReply(context, pattern.UnmarkBackstitches(command.Backstitches));
                    break;
            }
        }

        private static Task Deleted(IContext context) => Actor.Done;

        private void ApplyEvent(Event @event)
        {
            switch (@event.Data)
            {
                case PatternCreated e:
                    pattern = new PatternAggregate();
                    pattern.Apply(e);
                    behavior.Become(Active);
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

        private async Task PersistAndReply(IContext context, IEvent @event)
        {
            await persistence.PersistEventAsync(@event);
            context.Sender.Tell(@event);
        }
    }
}
