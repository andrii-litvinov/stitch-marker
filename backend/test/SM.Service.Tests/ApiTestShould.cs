using FluentAssertions;
using Xunit;

namespace SM.Service.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using Proto.Persistence;

    using SM.Service.Infrastructure;
    using SM.Service.Infrastructure.EventStore;
    using SM.Service.Patterns;

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
                var result = response.StatusCode;

                result.Should().Be(401);
            }

        }

        [Fact]
        public async void ApiTestShouldNotReturn401Error()
        {
            RequestBuilder req = new RequestBuilder(this.server, "http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            req.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJEUTFOVVkxT1RsR09EQXlRelV4TVRSR1JUazRSalpHUlRJNE1FUXhPVGxETkVNME9USTBOZyJ9.eyJpc3MiOiJodHRwczovL3N0aXRjaC1tYXJrZXIuYXV0aDAuY29tLyIsInN1YiI6IkFaSHJxSjRRdTJ0ZlowRjRveGxqQnRhTFN2M2NKUUQxQGNsaWVudHMiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAvYXBpLyIsImlhdCI6MTUyNTQ1ODQwMiwiZXhwIjoxNTI1NTQ0ODAyLCJhenAiOiJBWkhycUo0UXUydGZaMEY0b3hsakJ0YUxTdjNjSlFEMSIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.ciJhvUm5ROunGpvUAQ6JhVnMgUgLu05v_5jF8jt-JmJK5BKrVeBjutBr7-80FE-0n7wb-f9YubxvK6KVtWEGe8KntBUsqm5UsMbSoIob6pP7zDNpnVlbDm2yjn_2_ABmNsiMJfGuXccdc5qJzN0s3uCLjWsuNOnlt2CZ0naTOsD6Vs3MYpcHTnTRpNM25SlC5iCGoHS_ZUK5sblid8M8R5vMxdf1PUNe7x4VHa9Px0LUEAH4lOUxJPsWWfNyZDYZkwAiJQlr7Ro6y3PI-NitDDL2bssBRWn_Q-ApgYuDShua71UD5k-e57fFc7sUxYAMQYF2ktY2Vh0KqFxO6qURrQ");
            var response = await req.GetAsync();
            var result = response.StatusCode;
            result.Should().NotBe(401);

            //using (var client = server.CreateClient())
            //{
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IlJEUTFOVVkxT1RsR09EQXlRelV4TVRSR1JUazRSalpHUlRJNE1FUXhPVGxETkVNME9USTBOZyJ9.eyJpc3MiOiJodHRwczovL3N0aXRjaC1tYXJrZXIuYXV0aDAuY29tLyIsInN1YiI6IkFaSHJxSjRRdTJ0ZlowRjRveGxqQnRhTFN2M2NKUUQxQGNsaWVudHMiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUwMDAvYXBpLyIsImlhdCI6MTUyNTQ1ODQwMiwiZXhwIjoxNTI1NTQ0ODAyLCJhenAiOiJBWkhycUo0UXUydGZaMEY0b3hsakJ0YUxTdjNjSlFEMSIsImd0eSI6ImNsaWVudC1jcmVkZW50aWFscyJ9.ciJhvUm5ROunGpvUAQ6JhVnMgUgLu05v_5jF8jt-JmJK5BKrVeBjutBr7-80FE-0n7wb-f9YubxvK6KVtWEGe8KntBUsqm5UsMbSoIob6pP7zDNpnVlbDm2yjn_2_ABmNsiMJfGuXccdc5qJzN0s3uCLjWsuNOnlt2CZ0naTOsD6Vs3MYpcHTnTRpNM25SlC5iCGoHS_ZUK5sblid8M8R5vMxdf1PUNe7x4VHa9Px0LUEAH4lOUxJPsWWfNyZDYZkwAiJQlr7Ro6y3PI-NitDDL2bssBRWn_Q-ApgYuDShua71UD5k-e57fFc7sUxYAMQYF2ktY2Vh0KqFxO6qURrQ");

            //    var response = await client.GetAsync("http://localhost:5000/api/patterns/e0de656c-33e7-4ce1-8d5f-118e880d3871");
            //    var result = response.StatusCode;

            //    //response.Headers.Location.ToString();
            //    result.Should().NotBe(401);
            //}

        }
    }
}
