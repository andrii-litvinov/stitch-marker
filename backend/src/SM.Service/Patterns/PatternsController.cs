using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SM.Core;

namespace SM.Service.Patterns
{
    [Route("api/[controller]")]
    public class PatternsController : Controller
    {
        private readonly IPatternReader patternReader;

        public PatternsController(IPatternReader patternReader)
        {
            this.patternReader = patternReader;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            using (var fileStream = new FileStream("Patterns/M198_Seaside beauty.xsd", FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var pattern = patternReader.Read(memoryStream);
                return Ok(pattern);
            }
        }
    }
}