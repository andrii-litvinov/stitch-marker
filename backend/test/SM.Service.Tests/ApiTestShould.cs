using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SM.Service.Tests
{
    public class AuthTestShould
    {
        [Fact]
        public async Task ApiShouldReturn401()
        {
            // Act
            var result = await MakePostRequest("");

            // Assert
            result.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task ApiShouldReturn201()
        {
            // Arrange
            var token = await GetAuthToken();

            // Act
            var result = await MakePostRequest(token);

            // Assert
            result.StatusCode.Should().Be(201);
        }

        private async Task<string> GetAuthToken()
        {
            using (var httpClient = new HttpClient())
            {
                var stringContent = new StringContent(
                    JsonConvert.SerializeObject(
                        new
                            {
                                client_id = "AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1",
                                client_secret = "GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn",
                                audience = "http://localhost:5000/api/",
                                grant_type = "client_credentials"
                            }),
                    Encoding.UTF8,
                    "application/json");
                var response = await httpClient.PostAsync("https://stitch-marker.auth0.com/oauth/token", stringContent);
                var resultContent = await response.Content.ReadAsStringAsync();
                return JObject.Parse(resultContent).SelectToken("access_token").Value<string>();
            }
        }

        private async Task<HttpResponseMessage> MakePostRequest(string token)
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                var fileContent = await File.ReadAllBytesAsync(@"Resources/M198_Seaside beauty.xsd");
                var fileStreamContent = new StreamContent(new MemoryStream(fileContent));
                fileStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "M198_Seaside beauty.xsd" };
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(fileStreamContent);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    return await client.PostAsync("/api/patterns/", formData);
                }
            }
        }
    }
}
