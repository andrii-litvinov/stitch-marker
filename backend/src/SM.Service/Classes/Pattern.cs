using System;
using System.Collections.Generic;
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
        private readonly Queue<PID> thumbnailQueue = new Queue<PID>();
        private PatternState state;
        private PID thumbnailGeneratorPid;


        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
            behavior = new Behavior();
            behavior.Become(New);
        }


        public Task ReceiveAsync(IContext context)
        {
            thumbnailGeneratorPid = context.Spawn(Actor.FromProducer(() => new ThumbnailDrawer()));
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
                    thumbnailQueue.Enqueue(context.Sender);
                    thumbnailGeneratorPid.Tell(state);
                    break;
                case Thumbnail _:
                    context.Tell(thumbnailQueue.Dequeue(), context.Message);
                    break;
            }
            return Actor.Done;
        }
    }
}
