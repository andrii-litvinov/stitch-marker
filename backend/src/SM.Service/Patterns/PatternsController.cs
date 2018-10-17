using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proto.Cluster;

namespace SM.Service.Patterns
{
    [ApiController, Authorize, Route("api/patterns")]
    public class PatternsController : ControllerBase
    {
        [HttpPost, Route("markbackstitches")]
        public async Task<IActionResult> MarkBackstitches(BackstitchActionData data)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{data.PatternId}", ActorKind.Pattern);
            var command = new MarkBackstitches();
            command.Backstitches.AddRange(data.Backstitches);
            var result = await pattern.RequestAsync<bool>(command, 10.Seconds());

            return Ok(result);
        }
        
        [HttpPost, Route("unmarkbackstitches")]
        public async Task<IActionResult> UnmarkBackstitches(BackstitchActionData data)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{data.PatternId}", ActorKind.Pattern);
            var command = new UnmarkBackstitches();
            command.Backstitches.AddRange(data.Backstitches);
            var result = await pattern.RequestAsync<bool>(command, 10.Seconds());

            return Ok(result);
        }
        
        [HttpPost, Route("markstitches")]
        public async Task<IActionResult> MarkStitches(StitchActionData data)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{data.PatternId}", ActorKind.Pattern);
            var command = new MarkStitches();
            command.Stitches.AddRange(data.Stitches);
            var result = await pattern.RequestAsync<bool>(command, 10.Seconds());

            return Ok(result);
        }
        
        [HttpPost, Route("unmarkstitches")]
        public async Task<IActionResult> UnmarkStitches(StitchActionData data)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{data.PatternId}", ActorKind.Pattern);
            var command = new UnmarkStitches();
            command.Stitches.AddRange(data.Stitches);
            var result = await pattern.RequestAsync<bool>(command, 10.Seconds());

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource<PatternItem>>>> Get(int skip = 0, int take = 10)
        {
            var patternItems = await GetUserPatternItems(User.GetUserId(), skip, take);
            var result = new List<Resource<PatternItem>>();

            foreach (var item in patternItems.Items)
            {
                var resource = new Resource<PatternItem>(item)
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = Url.Action("Get", new {patternId = new Guid(item.Id)})},
                        new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {patternId = new Guid(item.Id)})}
                    }
                };

                result.Add(resource);
            }

            return result;
        }

        [HttpGet, Route("{patternId}")]
        public async Task<ActionResult<Service.Pattern>> Get(string patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId};
            var owner = await pattern.RequestAsync<PatternOwner>(query, 10.Seconds());

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            return await pattern.RequestAsync<Service.Pattern>(new GetPattern {Id = patternId}, 10.Seconds());
        }

        [HttpDelete, Route("{patternId}")]
        public async Task<IActionResult> Delete(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId.ToString()};
            var owner = await pattern.RequestAsync<PatternOwner>(query, 10.Seconds());

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            await pattern.RequestAsync<PatternDeleted>(new DeletePattern {Id = patternId.ToString()}, 10.Seconds());

            return Ok();
        }

        [HttpGet, Route("{patternId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid patternId, int width = 300, int height = 200)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var queryOwner = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId.ToString()};
            var owner = await pattern.RequestAsync<PatternOwner>(queryOwner, 10.Seconds());

            if (owner.OwnerId != User.GetUserId()) return Forbid();

            var query = new GetThumbnail {Id = Guid.NewGuid().ToString(), Height = height, Width = width};
            var thumbnail = await pattern.RequestAsync<Thumbnail>(query, 10.Seconds());
            return File(thumbnail.Image.ToByteArray(), "image/png");
        }

        [HttpPost]
        public async Task<ActionResult<Resource<PatternItem>>> Post(IFormFile file)
        {
            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            var patternId = Guid.NewGuid();
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId.ToString()}", ActorKind.Pattern);
            var content = await file.ReadAllBytes();
            var command = new CreatePattern
            {
                FileName = file.FileName,
                Id = patternId.ToString(),
                Content = ByteString.CopyFrom(content),
                OwnerId = userId
            };
            var @event = await pattern.RequestAsync<PatternCreated>(command, 10.Seconds());
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
            var resource = new Resource<PatternItem>(item)
            {
                Links =
                {
                    new Link {Rel = "self", Href = Url.Action("Get", new {patternId})},
                    new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {patternId})}
                }
            };

            return Created(patternId.ToString(), resource);
        }

        private static async Task<PatternItems> GetUserPatternItems(string userId, int skip, int take)
        {
            var (patternsByOwnerProjection, _) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection);
            var query = new GetPatternItems {RequestId = Guid.NewGuid().ToString(), OwnerId = userId, Skip = skip, Take = take};
            var cts = new CancellationTokenSource(1.Minutes());

            while (!cts.IsCancellationRequested)
            {
                var response = await patternsByOwnerProjection.RequestAsync<object>(query, 10.Seconds());
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
    }
}
