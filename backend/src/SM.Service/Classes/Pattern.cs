using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Proto;
using SM.Core;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly Behavior behavior;
        private readonly ThumbnailDrawer drawer;
        private readonly IPatternReader patternReader;
        private PatternState state;

        public Pattern(IPatternReader patternReader, ThumbnailDrawer drawer)
        {
            this.patternReader = patternReader;
            this.drawer = drawer;
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
                    context.Respond(new Thumbnail
                    {
                        Image = drawer.Draw(state)
                    });
                    break;
            }
            return Actor.Done;
        }
    }
}
