using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Service.Tests.Patterns
{
    public partial class PatterResourceTests
    {
        private static async Task<MultipartFormDataContent> GetFileContent(string fileName)
        {
            var fileContent = await File.ReadAllBytesAsync(fileName);
            var fileStreamContent = new ByteArrayContent(fileContent);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = "M198_Seaside beauty.xsd"
            };
            var formData = new MultipartFormDataContent {fileStreamContent};
            return formData;
        }

        private static async Task<string> GetAuthToken()
        {
            using (var httpClient = new HttpClient())
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(new
                {
                    client_id = "AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1",
                    client_secret = "GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn",
                    audience = "http://localhost:5000/api/",
                    grant_type = "client_credentials"
                }), Encoding.UTF8, "application/json");
                var response =
                    await httpClient.PostAsync("https://stitch-marker.auth0.com/oauth/token", stringContent);
                var resultContent = await response.Content.ReadAsStringAsync();
                return JObject.Parse(resultContent).SelectToken("access_token").Value<string>();
            }
        }
    }
}
