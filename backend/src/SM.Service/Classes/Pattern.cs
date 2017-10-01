using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proto;
using SM.Core;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly Behavior behavior;
        private readonly IPatternReader patternReader;
        private readonly Queue<WaitlistItem> thumbnailWaitlist = new Queue<WaitlistItem>();
        private PatternState state;

        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
            behavior = new Behavior();
            behavior.Become(New);
        }

        public Task ReceiveAsync(IContext context)
        {
            Console.WriteLine($"Message received {context.Message.GetType().Name}.");
            try
            {
                return behavior.ReceiveAsync(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Task New(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern command:
                    state = patternReader.Read(command.Content);
                    state.PatternId = command.PatternId;
                    context.Respond(new PatternBasicInfo
                    {
                        PatternId = state.PatternId,
                        PatternName = command.FileName,
                        Width = state.Width,
                        Height = state.Height
                    });
                    behavior.Become(Created);
                    break;
            }
            return Actor.Done;
        }

        private Task Created(IContext context)
        {
            switch (context.Message)
            {
                case PatternQuery _:
                    context.Respond(state);
                    break;
                case ThumbnailQuery _:
                    var drawer =
                        context.Children.FirstOrDefault(pid => pid.Id.EndsWith(nameof(ThumbnailDrawer))) ??
                        context.SpawnNamed(Actor.FromProducer(() => new ThumbnailDrawer()), nameof(ThumbnailDrawer));

                    var command = new CreateThumbnail {Id = Guid.NewGuid(), Pattern = state};
                    drawer.Tell(command);
                    thumbnailWaitlist.Enqueue(new WaitlistItem {Id = command.Id, Pid = context.Sender});
                    break;
                case Thumbnail thumbnail:
                    while (thumbnailWaitlist.TryDequeue(out var item))
                    {
                        if (thumbnail.Id == item.Id)
                        {
                            context.Tell(item.Pid, thumbnail);
                            break;
                        }
                    }
                    break;
            }
            return Actor.Done;
        }

        class WaitlistItem
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
