using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using RestSharp;

namespace SM.Service.Tests
{
    public class ApiTestShould
    {
        [Fact]
        public void ApiTestShouldReturn401Error()
        {
            var client = new RestClient("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            var request = new RestRequest(Method.GET);
            client.Execute(request).StatusCode.Should().Be(401);
        }

        class Token
        {
            public string access_token;
            public string expires_in;
            public string token_type;
        }

        [Fact]
        public void ApiTestShouldNotReturn401Error()
        {
            var client = new RestClient("https://stitch-marker.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", "{\"client_id\":\"AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1\",\"client_secret\":\"GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn\",\"audience\":\"http://localhost:5000/api/\",\"grant_type\":\"client_credentials\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            var token = JsonConvert.DeserializeObject<Token>(response.Content).access_token;

            client = new RestClient("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            request = new RestRequest(Method.GET);
            request.AddHeader("authorization", "Bearer " + token);
            client.Execute(request).StatusCode.Should().Be(500);
        }
    }
}
