﻿using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Proto;
using Proto.Persistence;
using Service.Patterns.Xsd;

namespace Service.Patterns
{
    public class PatternActor : IActor
    {
        private readonly Behavior behavior = new Behavior();
        private readonly IEventStore eventStore;
        private readonly MemoryCache senders = new MemoryCache(new MemoryCacheOptions());
        private PatternAggregate pattern = new PatternAggregate();
        private Persistence persistence;

        public PatternActor(IEventStore eventStore) => this.eventStore = eventStore;

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    context.SetReceiveTimeout(5.Minutes());
                    behavior.Become(Started);
                    persistence = Persistence.WithEventSourcing(eventStore, context.Self.Id, ApplyEvent);
                    await persistence.RecoverStateAsync();
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
                    // TODO [AL]: Use request id or trace id from headers to find sender.
                    senders.Set(command.Id, context.Sender, 30.Seconds());
                    context.Send(context.GetChild<XsdPatternActor>(), command);
                    break;
                case PatternCreated @event:
                    await persistence.PersistEventAsync(@event);
                    var sender = senders.Get<PID>(@event.SourceId);
                    if (sender != null)
                        context.Send(sender, @event);
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
                    senders.Set(query.Id, context.Sender, 30.Seconds());
                    context.Send(context.GetChild<PatternImageActor>(), query);
                    break;
                case GetPatternOwner _:
                    context.Respond(pattern.GetPatternOwner());
                    break;
                case Thumbnail thumbnail:
                    var sender = senders.Get<PID>(thumbnail.Id);
                    if (sender != null)
                        context.Send(sender, thumbnail);
                    break;
                case DeletePattern _:
                    await PersistAndRespond(context, pattern.Delete());
                    break;
                case MarkStitches command:
                    await PersistAndRespond(context, pattern.MarkStitches(command.Stitches));
                    break;
                case UnmarkStitches command:
                    await PersistAndRespond(context, pattern.UnmarkStitches(command.Stitches));
                    break;
                case MarkBackstitches command:
                    await PersistAndRespond(context, pattern.MarkBackstitches(command.Backstitches));
                    break;
                case UnmarkBackstitches command:
                    await PersistAndRespond(context, pattern.UnmarkBackstitches(command.Backstitches));
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

        private async Task PersistAndRespond(IContext context, IEvent @event)
        {
            await persistence.PersistEventAsync(@event);
            context.Respond(@event);
        }
    }
}
