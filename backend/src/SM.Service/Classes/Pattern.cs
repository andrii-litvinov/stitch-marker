using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using Proto.Persistence;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly Behavior behavior;
        private readonly IEventStore eventStore;
        private readonly Queue<WaitlistItem> patternWaitlist = new Queue<WaitlistItem>();
        private readonly Queue<WaitlistItem> thumbnailWaitlist = new Queue<WaitlistItem>();
        private PatternState pattern;
        private Persistence persistence;

        public Pattern(IEventStore eventStore)
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

        private void ApplyEvent(Event obj)
        {
        }

        private Task New(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    var parser =
                        context.Children.FirstOrDefault(pid => pid.Id.EndsWith(nameof(XsdPatternParser))) ??
                        context.SpawnNamed(Actor.FromProducer(() => new XsdPatternParser()), nameof(XsdPatternParser));
                    parser.Tell(command);
                    patternWaitlist.Enqueue(new WaitlistItem {Id = command.PatternId, Pid = context.Sender});
                    break;
                case PatternState patternState:
                    while (patternWaitlist.TryDequeue(out var item))
                        if (patternState.PatternId == item.Id)
                        {
                            pattern = patternState;
                            var info = new PatternBasicInfo
                            {
                                Height = patternState.Height,
                                Width = patternState.Width,
                                Title = patternState.Info.Title,
                                Id = patternState.PatternId
                            };
                            context.Tell(item.Pid, info);
                            behavior.Become(Created);
                        }
                    break;
            }
            return Actor.Done;
        }

        private Task Created(IContext context)
        {
            switch (context.Message)
            {
                case PatternQuery _:
                    context.Respond(pattern);
                    break;
                case ThumbnailQuery _:
                    var drawer =
                        context.Children.FirstOrDefault(pid => pid.Id.EndsWith(nameof(ThumbnailDrawer))) ??
                        context.SpawnNamed(Actor.FromProducer(() => new ThumbnailDrawer()), nameof(ThumbnailDrawer));

                    var command = new CreateThumbnail {Id = Guid.NewGuid(), Pattern = pattern};
                    drawer.Tell(command);
                    thumbnailWaitlist.Enqueue(new WaitlistItem {Id = command.Id, Pid = context.Sender});
                    break;
                case Thumbnail thumbnail:
                    while (thumbnailWaitlist.TryDequeue(out var item))
                        if (thumbnail.Id == item.Id)
                        {
                            context.Tell(item.Pid, thumbnail);
                            break;
                        }
                    break;
            }
            return Actor.Done;
        }

        private class WaitlistItem
        {
            public Guid Id { get; set; }
            public PID Pid { get; set; }
        }
    }

    public class CreateThumbnail
    {
        public Guid Id { get; set; }
        public PatternState Pattern { get; set; }
    }
}
