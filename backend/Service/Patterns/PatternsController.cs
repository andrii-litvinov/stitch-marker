using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proto;
using Proto.Cluster;
using SM.Service.Command;

namespace SM.Service.Patterns
{
    [ApiController, Authorize, Route("api/patterns")]
    public class PatternsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource<PatternItem>>>> Get(int skip = 0, int take = 10)
        {
            var patternItems = await GetUserPatternItems(User.GetUserId(), skip, take);
            return patternItems.Items.Select(CreatePatternResource).ToList();
        }

        [HttpGet, Route("{patternId}")]
        public async Task<ActionResult<Pattern>> Get(string patternId)
        {
            var pattern = await GetPattern(patternId);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId};
            var owner = await pattern.Request<PatternOwner>(query);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            return await pattern.Request<Pattern>(new GetPattern {Id = patternId});
        }

        [HttpDelete, Route("{patternId}")]
        public async Task<IActionResult> Delete(string patternId)
        {
            var pattern = await GetPattern(patternId);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId};
            var owner = await pattern.Request<PatternOwner>(query);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            await pattern.Request<PatternDeleted>(new DeletePattern {Id = patternId});

            return Ok();
        }

        [HttpGet, Route("{patternId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(string patternId, int width = 300, int height = 200)
        {
            var pattern = await GetPattern(patternId);
            var queryOwner = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId};
            var owner = await pattern.Request<PatternOwner>(queryOwner);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            var query = new GetThumbnail {Id = Guid.NewGuid().ToString(), Height = height, Width = width};
            var thumbnail = await pattern.Request<Thumbnail>(query);
            return File(thumbnail.Image.ToByteArray(), "image/png");
        }

        [HttpPost]
        public async Task<ActionResult<Resource<PatternItem>>> Post(IFormFile file)
        {
            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            var content = await file.ReadAllBytes();
            var command = new CreatePattern
            {
                FileName = file.FileName,
                Id = Guid.NewGuid().ToString(),
                Content = ByteString.CopyFrom(content),
                OwnerId = userId
            };
            var pattern = await GetPattern(command.Id);
            var @event = await pattern.Request<PatternCreated>(command);
            var item = new PatternItem
            {
                Id = @event.SourceId,
                Title = @event.Pattern.Info.Title,
                Height = @event.Pattern.Height,
                Width = @event.Pattern.Width,
                Author = @event.Pattern.Info.Author,
                Company = @event.Pattern.Info.Company,
                Copyright = @event.Pattern.Info.Copyright
            };

            return Created(@event.SourceId, CreatePatternResource(item));
        }

        [Route("{patternId}/mark-backstitches"), HttpPost]
        public async Task<ActionResult<BackstitchesMarked>> MarkBackstitches(MarkBackstitches command)
        {
            var pattern = await GetPattern(command.PatternId);
            return await pattern.Request<BackstitchesMarked>(command);
        }

        [Route("{patternId}/unmark-backstitches"), HttpPost]
        public async Task<ActionResult<BackstitchesUnmarked>> UnmarkBackstitches(UnmarkBackstitches command)
        {
            var pattern = await GetPattern(command.PatternId);
            return await pattern.Request<BackstitchesUnmarked>(command);
        }

        [Route("{patternId}/mark-stitches"), HttpPost]
        public async Task<ActionResult<StitchesMarked>> MarkStitches(MarkStitches command)
        {
            var pattern = await GetPattern(command.PatternId);
            return await pattern.Request<StitchesMarked>(command);
        }

        [Route("{patternId}/unmark-stitches"), HttpPost]
        public async Task<ActionResult<StitchesUnmarked>> UnmarkStitches(UnmarkStitches command)
        {
            var pattern = await GetPattern(command.PatternId);
            return await pattern.Request<StitchesUnmarked>(command);
        }

        private static async Task<PatternItems> GetUserPatternItems(string userId, int skip, int take)
        {
            var (patternsByOwnerProjection, _) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection);
            var query = new GetPatternItems {RequestId = Guid.NewGuid().ToString(), OwnerId = userId, Skip = skip, Take = take};
            var cts = new CancellationTokenSource(1.Minutes());

            while (!cts.IsCancellationRequested)
            {
                var response = await patternsByOwnerProjection.Request<object>(query);
                switch (response)
                {
                    case PatternItems items:
                        return items;
                    case CatchingUp _:
                        await Task.Delay(100, cts.Token);
                        break;
                    default:
                        throw new Exception($"Unexpected Response of type {response.GetType().Name} received.");
                }
            }

            throw new TimeoutException("Request didn't receive expected Response within the expected time.");
        }

        private static async Task<PID> GetPattern(string patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            return pattern;
        }

        private Resource<PatternItem> CreatePatternResource(PatternItem item) => new Resource<PatternItem>(item)
        {
            Links =
            {
                new Link {Rel = "self", Href = Url.Action("Get", new {patternId = new Guid(item.Id)})},
                new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {patternId = new Guid(item.Id)})},
                new Link {Rel = "mark-stitches", Href = Url.Action("MarkStitches", new {patternId = new Guid(item.Id)})},
                new Link {Rel = "unmark-stitches", Href = Url.Action("UnmarkStitches", new {patternId = new Guid(item.Id)})},
                new Link {Rel = "mark-backstitches", Href = Url.Action("MarkBackstitches", new {patternId = new Guid(item.Id)})},
                new Link {Rel = "unmark-backstitches", Href = Url.Action("UnmarkBackstitches", new {patternId = new Guid(item.Id)})}
            }
        };
    }

    public static class PidExtensions
    {
        public static async Task<TResponse> Request<TResponse>(this PID pid, object message) =>
            await pid.RequestAsync<TResponse>(message, 10.Seconds());
    }
}
