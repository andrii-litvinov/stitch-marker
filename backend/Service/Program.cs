﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto;
using Proto.Persistence;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Build().Run();
        }

        public static IWebHostBuilder BuildWebHost(params string[] args) => WebHost
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => { builder.AddEnvironmentVariables("STITCH_MARKER:"); })
            .ConfigureServices((context, services) =>
            {
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                services.AddCors();
                services.AddSingleton<IHostedService, ActorCluster>();
                services.AddSingleton<IEventStore, EventStore>();
                services.AddSingleton<ISubscriptionEventStoreConnection, StreamSubscriberConnection>(
                    provider => new StreamSubscriberConnection(context.Configuration["EVENTSTORE_CONNECTION"]));
                services.AddSingleton<IReadWriteEventStoreConnection, ReadWriteEventStoreConnection>(
                    provider => new ReadWriteEventStoreConnection(context.Configuration["EVENTSTORE_CONNECTION"]));
                services.AddSingleton<IActorFactory, ActorFactory>();

                services
                    .AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.Authority = context.Configuration["AUTH_AUTHORITY"];
                        options.Audience = context.Configuration["AUTH_AUDIENCE"];
                    });

                services.AddTransient<ISenderContext, RootContext>();
            })
            .Configure(app =>
            {
                app.UseAuthentication();
                app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Location"));
                app.UseMvc();
            });
    }
}
