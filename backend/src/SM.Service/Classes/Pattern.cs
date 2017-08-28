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
        private PatternState state;

        public Pattern(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
            behavior = new Behavior();
            behavior.Become(New);
        }

        public Task ReceiveAsync(IContext context)
        {
            return behavior.ReceiveAsync(context);
        }

        private Task New(IContext context)
        {
            switch (context.Message)
            {
                case CreatePattern pattern:
                    state = patternReader.Read(pattern.Content);
                    context.Respond(new PatternBasicInfo
                    {
                        PatternName = state.Info.Title,
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
            }
            return Actor.Done;
        }
    }
}
