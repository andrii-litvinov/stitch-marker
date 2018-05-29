using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;
using SM.Service.Extensions;
using SM.Service.Messages;
using SM.Service.Resources;

namespace SM.Service.Patterns
{
    [Authorize]
    [Route("api/patterns")]
    public class PatternsController : Controller
    {
        [HttpGet]
        [Route("{patternId}")]
        public async Task<IActionResult> Get(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            dynamic response = await pattern.RequestAsync<PatternOwner>(new GetPatternOwner {Id = patternId.ToString()}, 10.Seconds());

            if (response.OwnerId != User.GetUserId()) return Forbid();

            response = await pattern.RequestAsync<Pattern>(new GetPattern {Id = patternId.ToString()}, 10.Seconds());

            return Ok(response);
        }

        [HttpDelete]
        [Route("{patternId}")]
        public async Task<IActionResult> Delete(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var response = await pattern.RequestAsync<PatternOwner>(new GetPatternOwner {Id = patternId.ToString()}, 10.Seconds());

            if (response.OwnerId != User.GetUserId()) return Forbid();

            await pattern.RequestAsync<PatternDeleted>(new DeletePattern {Id = patternId.ToString()}, 10.Seconds());

            return Ok();
        }

        [HttpGet]
        [Route("{patternId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid patternId, int width = 300, int height = 200)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var response = await pattern.RequestAsync<PatternOwner>(new GetPatternOwner {Id = patternId.ToString()}, 10.Seconds());

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
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId.ToString()}", "pattern");
            var content = await file.ReadAllBytes();
            var command = new CreatePattern
            {
                FileName = file.FileName,
                Id = patternId.ToString(),
                Content = ByteString.CopyFrom(content),
                OwnerId = userId
            };
            var @event = await pattern.RequestAsync<PatternCreated>(command, 10.Seconds());
            var preview = new {@event.Id, @event.Pattern.Info.Title, @event.Pattern.Height, @event.Pattern.Width};
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
