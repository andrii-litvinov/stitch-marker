using System;
using System.IO;
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
        [HttpGet("{patternId}")]
        public async Task<IActionResult> Get(Guid patternId)
        {
            var pid = await Cluster.GetAsync($"{patternId}", "Pattern");
            var request = await pid.RequestAsync<PatternState>(new PatternQuery());
            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var patternId = Guid.NewGuid();
            var pid = await Cluster.GetAsync($"{patternId}", "Pattern");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var createPattern = new CreatePattern(patternId.ToString(), memoryStream.ToArray());
                pid.Tell(createPattern);
            }

            return Created(patternId.ToString(), null);
        }
    }
}