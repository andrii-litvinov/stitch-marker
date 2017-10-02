using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;
using SM.Core.Model;
using SM.Service.Classes;

namespace SM.Service.Patterns
{
    [Route("api/patterns")]
    public class PatternsController : Controller
    {
        [Route("{patternId}")]
        public async Task<IActionResult> Get(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var request = await pattern.RequestAsync<PatternState>(new PatternQuery(), 3.Seconds());
            return Ok(request);
        }

        [Route("{patternId}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(Guid patternId)
        {
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var request = await pattern.RequestAsync<Thumbnail>(new ThumbnailQuery(), 3.Seconds());
            return File(request.Image, "image/png");
        }

        public async Task<IActionResult> Post(IFormFile file)
        {
            var patternId = Guid.NewGuid();
            var (pattern, _) = await Cluster.GetAsync($"pattern-{patternId.ToString()}", "pattern");
            var content = await file.ReadAllBytes();
            var command = new CreatePattern(patternId, file.FileName, content);
            var info = await pattern.RequestAsync<PatternPreview>(command, 3.Seconds());
            var resource = new Resource(info)
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
