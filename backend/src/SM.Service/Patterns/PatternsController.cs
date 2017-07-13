using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SM.Core;

namespace SM.Service.Patterns
{
    [Route("api/[controller]")]
    public class PatternsController : Controller
    {
        private readonly IPatternReader patternReader;
        private readonly ILogger logger;

        public PatternsController(IPatternReader patternReader, ILoggerFactory loggerFactory)
        {
            this.patternReader = patternReader;
            this.logger = loggerFactory.CreateLogger("My");
        }

        [HttpGet("{patternId}")]
        public IActionResult Get(Guid patternId)
        {
            var path = $"Patterns/{patternId}/pattern.json";
            logger.LogInformation(path);
            if (!System.IO.File.Exists(path)) return NotFound();

            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.Asynchronous);
            return File(fileStream, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var patternId = Guid.NewGuid();
            var path = $"Patterns/{patternId}";
            Directory.CreateDirectory(path);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                using (var fileStream = new FileStream($"{path}/pattern.xsd", FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                {
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(fileStream);
                }

                memoryStream.Position = 0;
                var pattern = patternReader.Read(memoryStream);
                var json = JsonConvert.SerializeObject(pattern,
                    new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

                using (var fileStream = new FileStream($"{path}/pattern.json", FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                using (var stringWriter = new StreamWriter(fileStream))
                {
                    await stringWriter.WriteAsync(json);
                }
            }

            return Created(patternId.ToString(), null);
        }
    }
}