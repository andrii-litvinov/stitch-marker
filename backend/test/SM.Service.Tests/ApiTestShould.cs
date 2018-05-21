using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using System.IO;

namespace SM.Service.Tests
{
    public class ApiTestShould
    {
        class Token
        {
            public string access_token;
            public string expires_in;
            public string token_type;
        }

        class AuthTokenRequestData
        {
            public string client_id = "AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1";
            public string client_secret = "GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn";
            public string audience = "http://localhost:5000/api/";
            public string grant_type = "client_credentials";
        }

        [Fact]
        public async void ApiTestShouldReturn401()
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                var response = await client.GetAsync("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
                response.StatusCode.Should().Be(401);
            }
        }

        [Fact]
        public async void ApiTestShouldReturn201()
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                using (var httpClient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(new AuthTokenRequestData()), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://stitch-marker.auth0.com/oauth/token", stringContent);
                    var resultContent = response.Content.ReadAsStringAsync().Result;
                    var token = JsonConvert.DeserializeObject<Token>(resultContent).access_token;

                    var fileStreamContent = new StreamContent(File.OpenRead(@"Resources/M198_Seaside beauty.xsd"));
                    fileStreamContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data") { Name = "file", FileName = "M198_Seaside beauty.xsd" };
                    fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.Add(fileStreamContent);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        response = await client.PostAsync("/api/patterns/", formData);
                        response.StatusCode.Should().Be(201);
                    }
                }
            }
        }
    }
}
