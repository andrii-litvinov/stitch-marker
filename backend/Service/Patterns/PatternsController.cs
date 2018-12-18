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

namespace Service.Patterns
{
    [ApiController, Authorize, Route("api/patterns")]
    public class PatternsController : ControllerBase
    {
        private readonly ISenderContext context;

        public PatternsController(ISenderContext context) => this.context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource<PatternItem>>>> Get(int skip = 0, int take = 10)
        {
            var patternItems = await GetUserPatternItems(User.GetUserId(), skip, take);
            return patternItems.Items.Select(CreatePatternResource).ToList();
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<Pattern>> Get(string id)
        {
            var pattern = await GetPattern(id);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = id};
            var owner = await context.Request<PatternOwner>(pattern, query);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            return await context.Request<Pattern>(pattern, new GetPattern {Id = id});
        }

        [HttpDelete, Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var pattern = await GetPattern(id);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = id};
            var owner = await context.Request<PatternOwner>(pattern, query);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            await context.Request<PatternDeleted>(pattern, new DeletePattern {Id = id});

            return Ok();
        }

        [HttpGet, Route("{id}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(string id, int width = 300, int height = 200)
        {
            var pattern = await GetPattern(id);
            var queryOwner = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = id};
            var owner = await context.Request<PatternOwner>(pattern, queryOwner);

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            var query = new GetThumbnail {Id = Guid.NewGuid().ToString(), Height = height, Width = width};
            var thumbnail = await context.Request<Thumbnail>(pattern, query);
            return File(thumbnail.Image, "image/png");
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
                Content = content,
                OwnerId = userId
            };
            var pattern = await GetPattern(command.Id);
            var @event = await context.Request<PatternCreated>(pattern, command);
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

        [Route("{id}/mark-backstitches"), HttpPut]
        public async Task<ActionResult<BackstitchesMarked>> MarkBackstitches(MarkBackstitches command)
        {
            var pattern = await GetPattern(command.Id);
            return await context.Request<BackstitchesMarked>(pattern, command);
        }

        [Route("{id}/unmark-backstitches"), HttpPut]
        public async Task<ActionResult<BackstitchesUnmarked>> UnmarkBackstitches(UnmarkBackstitches command)
        {
            var pattern = await GetPattern(command.Id);
            return await context.Request<BackstitchesUnmarked>(pattern, command);
        }

        [Route("{id}/mark-stitches"), HttpPut]
        public async Task<ActionResult<StitchesMarked>> MarkStitches(MarkStitches command)
        {
            var pattern = await GetPattern(command.Id);
            return await context.Request<StitchesMarked>(pattern, command);
        }

        [Route("{id}/unmark-stitches"), HttpPut]
        public async Task<ActionResult<StitchesUnmarked>> UnmarkStitches(UnmarkStitches command)
        {
            var pattern = await GetPattern(command.Id);
            return await context.Request<StitchesUnmarked>(pattern, command);
        }

        private async Task<PatternItems> GetUserPatternItems(string userId, int skip, int take)
        {
            var (patternsByOwnerProjection, _) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection);
            var query = new GetPatternItems {RequestId = Guid.NewGuid().ToString(), OwnerId = userId, Skip = skip, Take = take};
            var cts = new CancellationTokenSource(1.Minutes());

            while (!cts.IsCancellationRequested)
            {
                var response = await context.Request<object>(patternsByOwnerProjection, query);
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

        private static async Task<PID> GetPattern(string id)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{id}", ActorKind.Pattern);
            return pattern;
        }

        private Resource<PatternItem> CreatePatternResource(PatternItem item) => new Resource<PatternItem>(item)
        {
            Links =
            {
                new Link {Rel = "self", Href = Url.Action("Get", new {id = new Guid(item.Id)})},
                new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {id = new Guid(item.Id)})},
                new Link {Rel = "mark-stitches", Href = Url.Action("MarkStitches", new {id = new Guid(item.Id)})},
                new Link {Rel = "unmark-stitches", Href = Url.Action("UnmarkStitches", new {id = new Guid(item.Id)})},
                new Link {Rel = "mark-backstitches", Href = Url.Action("MarkBackstitches", new {id = new Guid(item.Id)})},
                new Link {Rel = "unmark-backstitches", Href = Url.Action("UnmarkBackstitches", new {id = new Guid(item.Id)})}
            }
        };
    }
}
