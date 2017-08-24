using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Proto.Cluster;
using SM.Core;
using SM.Service.Classes;

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

        [HttpGet("{patternId}")]
        public IActionResult Get(Guid patternId)
        {
            var path = $"Patterns/{patternId}/pattern.json";
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
            var pid = await Cluster.GetAsync($"{patternId}", "Pattern");

            using (var memoryStream1 = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream1);

                using (var fileStream = new FileStream($"{path}/pattern.xsd", FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
                {
                    memoryStream1.Position = 0;
                    await memoryStream1.CopyToAsync(fileStream);
                }
                var bytes = memoryStream1.ToArray();
                var createPattern = new CreatePattern(patternId.ToString(), bytes);
                var reply = await pid.RequestAsync<object>(createPattern);
            }

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