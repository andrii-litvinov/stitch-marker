using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Proto.Persistence;
using SM.Service.Infrastructure;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddEnvironmentVariables("STITCH_MARKER:");
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddMvc();
                    services.AddCors();
                    services.AddSingleton<IHostedService, ActorCluster>();
                    services.AddSingleton<IEventStore, Infrastructure.EventStore.EventStore>();
                    services.AddSingleton<IReadWriteEventStoreConnection, ReadWriteEventStoreConnection>(
                        provider => new ReadWriteEventStoreConnection(context.Configuration["EVENTSTORE_CONNECTION"]));
                })
                .Build()
                .Run();
        }
    }
}
