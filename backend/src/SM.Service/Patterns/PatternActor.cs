using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;
using SM.Service.Extensions;
using SM.Service.Messages;
using SM.Service.Patterns.Xsd;

namespace SM.Service.Patterns
{
    public class PatternActor : IActor
    {
        private readonly Behavior behavior;
        private readonly IEventStore eventStore;
        private readonly MemoryCache recipients = new MemoryCache(new MemoryCacheOptions());
        private Messages.Pattern pattern;
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
                    persistence = Persistence.WithEventSourcing(eventStore, context.Self.Id, ApplyEvent);
                    await persistence.RecoverStateAsync();
                    if (pattern == null) behavior.Become(New);
                    else behavior.Become(Created);
                    break;
                default:
                    await behavior.ReceiveAsync(context);
                    break;
            }
        }

        private Task New(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    var parser = context.GetChild<XsdPatternActor>();
                    recipients.Set(command.Id, context.Sender, 30.Seconds());
                    parser.Tell(command);
                    break;
                case Messages.Pattern pattern1:
                    pattern = pattern1;
                    // TODO: Save state.
                    behavior.Become(Created);
                    var preview = new PatternPreview
                    {
                        Id = pattern.Id,
                        Title = pattern.Info.Title,
                        Width = pattern.Width,
                        Height = pattern.Height
                    };
                    recipients.Get<PID>(pattern.Id)?.Tell(preview);
                    break;
            }
            return Actor.Done;
        }

        private Task Created(IContext context)
        {
            switch (context.Message)
            {
                case GetPattern _:
                    context.Respond(pattern);
                    break;
                case GetThumbnail query:
                    query.Pattern = pattern;
                    var drawer = context.GetChild<PatternImageActor>();
                    recipients.Set(query.Id, context.Sender, 30.Seconds());
                    drawer.Tell(query);
                    break;
                case Thumbnail thumbnail:
                    recipients.Get<PID>(thumbnail.Id)?.Tell(thumbnail);
                    break;
            }
            return Actor.Done;
        }

        private void ApplyEvent(Event @event)
        {
        }
    }
}
