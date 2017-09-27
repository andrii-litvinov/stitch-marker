using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proto.Cluster;
using SM.Core.Model;
using SM.Service.Classes;

namespace SM.Service.Patterns
{
    [Route("api/[controller]")]
    public class PatternsController : Controller
    {
        [Route("{patternId}")]
        public async Task<IActionResult> Get(Guid patternId)
        {
            var pattern = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var request = await pattern.RequestAsync<PatternState>(new PatternQuery());
            return Ok(request);
        }

        [Route("{patternId}/thumbnail")]
        public async Task<IActionResult> Get(Guid patternId, int width, int height)
        {
            var pattern = await Cluster.GetAsync($"pattern-{patternId}", "pattern");
            var request = await pattern.RequestAsync<Thumbnail>(new ThumbnailQuery(), 3.Seconds());
            return File(request.Image, "image/png");
        }

        public async Task<IActionResult> Post(IFormFile file)
        {
            var patternId = Guid.NewGuid();
            var pattern = await Cluster.GetAsync($"pattern-{patternId.ToString()}", "pattern");
            var content = await file.ReadAllBytes();
            var command = new CreatePattern(patternId, file.FileName, content);
            var info = await pattern.RequestAsync<PatternBasicInfo>(command, 3.Seconds());
            return Created(patternId.ToString(), info);
        }
    }
}
