using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;
using SM.Service.Extensions;
using SM.Service.Messages;
using SM.Service.Patterns.Xsd;
using Pattern = SM.Service.Messages.Pattern;

namespace SM.Service.Patterns
{
    public class PatternActor : IActor
    {
        private readonly Behavior behavior;
        private readonly IEventStore eventStore;
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());
        private Pattern pattern;
        private Persistence persistence;

        public PatternActor(IEventStore eventStore)
        {
            this.eventStore = eventStore;
            behavior = new Behavior();
        }

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
                    await persistence.PersistEventAsync(
                        new PatternUploaded {Id = command.Id, FileName = command.FileName, Content = command.Content, UserId = command.UserId});
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
                    var patternDeleted = new PatternDeleted {Id = command.Id};
                    await persistence.PersistEventAsync(patternDeleted);
                    context.Sender.Tell(patternDeleted);
                    break;
            }
        }

        private Task Deleted(IContext context)
        {
            return Actor.Done;
        }
        
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
    }
}
