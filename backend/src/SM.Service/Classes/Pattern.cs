using System;
using System.Threading.Tasks;
using Proto;
using SM.Core;
using SM.Core.Model;

namespace SM.Service.Classes
{
    public class Pattern : IActor
    {
        private readonly IPatternReader patternReader;
        private readonly Behavior behavior;
        private PatternState state;

        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
            behavior = new Behavior();
            behavior.Become(Empty);
        }

        public Task ReceiveAsync(IContext context)
        {
            return behavior.ReceiveAsync(context);
        }

        private Task Empty(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern pattern:
                    state = patternReader.Read(pattern.Content);
                    behavior.Become(Created);
                    break;
                case PatternQuery _:
                    break;
            }
            return Actor.Done;
        }

        private Task Created(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern pattern:
                    break;
                case PatternQuery _:
                    context.Respond(state);
                    break;
            }
            return Actor.Done;
        }
    }
}
