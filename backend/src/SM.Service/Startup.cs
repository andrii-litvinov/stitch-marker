using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto.Persistence;
using SM.Service.Infrastructure;
using SM.Service.Infrastructure.EventStore;

namespace SM.Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddCors();
            services.AddSingleton<IHostedService, ActorCluster>();
            services.AddSingleton<IEventStore, Infrastructure.EventStore.EventStore>();
            services.AddSingleton<IReadWriteEventStoreConnection, ReadWriteEventStoreConnection>(
                provider => new ReadWriteEventStoreConnection("ConnectTo=tcp://admin:changeit@localhost:1113"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Location"));

            app.UseMvc();
        }
    }
}
