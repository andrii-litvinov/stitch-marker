using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

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

        class AuthTokenRequest
        {
            public string client_id = @"AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1";
            public string client_secret = @"GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn";
            public string audience = @"http://localhost:5000/api/";
            public string grant_type = @"client_credentials";
        }

        [Fact]
        public async void ApiTestShouldReturn401Error()
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                var response = await client.GetAsync("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
                response.StatusCode.Should().Be(401);
            }
        }

        [Fact]
        public async void ApiTestShouldNotReturn401Error()
        {
            using (var client = new TestServer(Program.BuildWebHost()).CreateClient())
            {
                using (var httpClient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(new AuthTokenRequest()), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("https://stitch-marker.auth0.com/oauth/token", stringContent);
                    var resultContent = response.Content.ReadAsStringAsync().Result;
                    var token = JsonConvert.DeserializeObject<Token>(resultContent).access_token;

                    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    response = await client.GetAsync("/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
                    response.StatusCode.Should().NotBe(401);
                }
            }
        }
    }
}
