using FluentAssertions;
using Xunit;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Proto.Persistence;

using SM.Service.Infrastructure;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service.Tests
{
    public class ApiTestShould
    {
        private readonly TestServer server;

        public ApiTestShould()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>()
                .ConfigureAppConfiguration(builde => { builde.AddEnvironmentVariables("STITCH_MARKER:"); })
                .ConfigureServices(
                    (context, services) =>
                        {
                            services.AddMvc();
                            services.AddCors();
                            services.AddSingleton<IHostedService, ActorCluster>();
                            services.AddSingleton<IEventStore, Infrastructure.EventStore.EventStore>();
                            services.AddSingleton<IReadWriteEventStoreConnection, ReadWriteEventStoreConnection>(
                                provider => new ReadWriteEventStoreConnection(
                                    context.Configuration["EVENTSTORE_CONNECTION"]));
                            services.AddAuthentication(
                                options =>
                                    {
                                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                                    }).AddJwtBearer(
                                options =>
                                    {
                                        options.Authority = context.Configuration["AUTH_AUTHORITY"];
                                        options.Audience = context.Configuration["AUTH_AUDIENCE"];
                                    });
                        });
          
            server = new TestServer(builder);
        }

        [Fact]
        public async void ApiTestShouldReturn401Error()
        {
            using (var client = server.CreateClient())
            {
                var response = await client.GetAsync("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
                response.StatusCode.Should().Be(401);
            }
        }

        class Token
        {
            public string access_token;
            public string expires_in;
            public string token_type;
        }

        [Fact]
        public async void ApiTestShouldNotReturn401Error()
        {
            var clientAuth = new RestClient("https://stitch-marker.auth0.com/oauth/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter(
                "application/json",
                "{\"client_id\":\"AZHrqJ4Qu2tfZ0F4oxljBtaLSv3cJQD1\",\"client_secret\":\"GsJR7FwkJ-fCQDnhhh3GtwGlM4svMLMzAWDMhCeJZuuzGHzDAhznbIriQ3FF4UNn\",\"audience\":\"http://localhost:5000/api/\",\"grant_type\":\"client_credentials\"}",
                ParameterType.RequestBody);

            IRestResponse response = clientAuth.Execute(request);
            var token = JsonConvert.DeserializeObject<Token>(response.Content).access_token;

            clientAuth = new RestClient("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + token);
            response = clientAuth.Execute(request);

            //using (var client = server.CreateClient())
            //{
            //    //var t = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJEUTFOVVkxT1RsR09EQXlRelV4TVRSR1JUazRSalpHUlRJNE1FUXhPVGxETkVNME9USTBOZyJ9.eyJpc3MiOiJodHRwczovL3N0aXRjaC1tYXJrZXIuYXV0aDAuY29tLyIsInN1YiI6IkFaSHJxSjRRdTJ0ZlowRjRveGxqQnRhTFN2M2NKUUQxQGNsaWVudHMiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAvYXBpLyIsImlhdCI6MTUyNjQ5NTUxMSwiZXhwIjoxNTI2NTgxOTExLCJhenAiOiJBWkhycUo0UXUydGZaMEY0b3hsakJ0YUxTdjNjSlFEMSIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.EXpGbgzTz-kDp90uF7_3XhWC6rIIC5vgFF1tE-wPp4t2cMnV9isod6FeSw9H4FqD6RRcAnX0d9RFa-MjJBbO779P1fvddBSQQcmE_1weTCvbOIuHQUuaJJ_BRU7HKI7b2HuyONFgQBbXLSHF1ZeodoLcV9Ly9_sghFF4OqEOyNtGxrl7ey88mC1pCssNCybLv5nQh7TfdsJnrgkqKuysQY77UrsYgtvTSYXgXgn-mhbVoPVJytfowiEZToYF-we5sRUCD-Hc2JtweX3tUx6IokM_Nzovv8_lsjLfYULOg9xO3cpJoT5HDal4UiVP6z-KcRM_kwvB5woDti8DiNlijg";
            //    AuthenticationHeaderValue authHeaders = new AuthenticationHeaderValue("Bearer", token);
            //    client.DefaultRequestHeaders.Authorization = authHeaders;
            //    var getResponse = await client.GetAsync("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            //}
            response.Should().NotBe(401);
        }
    }
}
