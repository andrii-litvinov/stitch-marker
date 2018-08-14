using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;

namespace SM.Service.Patterns
{
    [Authorize, Route("api/patterns")]
    public class PatternsController : Controller
    {
        [HttpGet, Route("getall")]
        public async Task<IActionResult> GetAllPatternItems()
        {
            var userId = User.GetUserId();
            if (userId == null) return BadRequest();

            var patterns = await GetUserPatternItems(userId);

            var result = new List<Resource>();
            foreach (var item in patterns.Items)
            {
                var patternId = new Guid(item.Id);

                var preview = new {item.Id, item.Title, item.Height, item.Width};
                var resource = new Resource(preview)
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = Url.Action("Get", new {patternId})},
                        new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {patternId})}
                    }
                };

                result.Add(resource);
            }

            return Ok(result);
        }

        private async Task<PatternItems> GetUserPatternItems(string userId)
        {
            var (patternsByOwnerProjection, _) = await Cluster.GetAsync(ActorKind.PatternsByOwnerProjection, ActorKind.PatternsByOwnerProjection);
            var query = new GetPatternItems {RequestId = Guid.NewGuid().ToString(), OwnerId = userId, Skip = 0, Take = 100};

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (stopWatch.Elapsed > 30.Seconds()) throw new TimeoutException("Retrieving user pattern items");
                var response = await patternsByOwnerProjection.RequestAsync<object>(query, 10.Seconds());
                switch (response)
                {
                    case PatternItems items:
                        stopWatch.Stop();
                        return items;
                    case CatchingUp _:
                        await Task.Delay(100);
                        break;
                    default:
                        throw new Exception("Unknown response type.");
                }
            }
        }

        [HttpGet, Route("{patternId}")]
        public async Task<IActionResult> Get(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId.ToString()};
            dynamic response = await pattern.RequestAsync<PatternOwner>(query, 10.Seconds());

            if (response.OwnerId != User.GetUserId()) return Forbid();

            response = await pattern.RequestAsync<Service.Pattern>(new GetPattern {Id = patternId.ToString()}, 10.Seconds());

            return Ok(response);
        }

        [HttpDelete, Route("{patternId}")]
        public async Task<IActionResult> Delete(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var query = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId.ToString()};
            var response = await pattern.RequestAsync<PatternOwner>(query, 10.Seconds());

            if (response.OwnerId != User.GetUserId()) return Forbid();

            await pattern.RequestAsync<PatternDeleted>(new DeletePattern {Id = patternId.ToString()}, 10.Seconds());

            return Ok();
        }

        [HttpGet, Route("{patternId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid patternId, int width = 300, int height = 200)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", ActorKind.Pattern);
            var queryOwner = new GetPatternOwner {RequestId = Guid.NewGuid().ToString(), PatternId = patternId.ToString()};
            var response = await pattern.RequestAsync<PatternOwner>(queryOwner, 10.Seconds());

            if (response.OwnerId != User.GetUserId()) return Forbid();

            var query = new GetThumbnail {Id = Guid.NewGuid().ToString(), Height = height, Width = width};
            var thumbnail = await pattern.RequestAsync<Thumbnail>(query, 10.Seconds());
            return File(thumbnail.Image.ToByteArray(), "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
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
            var preview = new {Id = @event.SourceId, @event.Pattern.Info.Title, @event.Pattern.Height, @event.Pattern.Width};
            var resource = new Resource(preview)
            {
                Links =
                {
                    new Link {Rel = "self", Href = Url.Action("Get", new {patternId})},
                    new Link {Rel = "thumbnail", Href = Url.Action("GetThumbnail", new {patternId})}
                }
            };

            return Created(patternId.ToString(), resource);
        }
    }
}
