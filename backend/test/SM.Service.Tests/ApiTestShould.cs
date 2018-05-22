using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SM.Service.Tests
{
    public class AuthTestShould
    {
        [Fact]
        public async Task ApiShouldReturn401()
        {
            // Act
            var result = await this.MakePostRequestAsync(string.Empty);

            // Assert
            result.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task ApiShouldReturn201()
        {
            // Arrange
            var token = await this.GetAuthTokenAsync();

            // Act
            var result = await this.MakePostRequestAsync(token);

            // Assert
            result.StatusCode.Should().Be(201);
        }

        async Task<string> GetAuthTokenAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var stringContent = new StringContent("{\"client_id\":\"AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1\",\"client_secret\":\"GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn\",\"audience\":\"http://localhost:5000/api/\",\"grant_type\":\"client_credentials\"}", Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://stitch-marker.auth0.com/oauth/token", stringContent);
                var resultContent = await response.Content.ReadAsStringAsync();
                return JObject.Parse(resultContent).SelectToken("access_token").Value<string>();
            }
        }

        async Task<HttpResponseMessage> MakePostRequestAsync(string token)
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                var fileStreamContent = new StreamContent(File.OpenRead(@"Resources/M198_Seaside beauty.xsd"));
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
