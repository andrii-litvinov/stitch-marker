using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using SM.Core;
using SM.Service.Classes;
using SM.Xsd;
using Pattern = SM.Service.Classes.Pattern;

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
            services.AddSingleton<IPatternReader, XsdPatternReader>();
            services.AddSingleton<IHostedService, ActorCluster>();
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

    public class ActorCluster : IHostedService
    {
        private readonly IConfiguration configuration;
        private readonly IPatternReader patternReader;

        public ActorCluster(IConfiguration configuration, IPatternReader patternReader)
        {
            this.configuration = configuration;
            this.patternReader = patternReader;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var drawer = new ThumbnailDrawer();
            var props = Actor.FromProducer(() => new Pattern(patternReader, drawer));

            // TODO: Register all known actors in a generic way 
            Remote.RegisterKnownKind("pattern", props);
            Remote.Start("127.0.0.1", 12001);
            Cluster.Start("PatternCluster", new ConsulProvider(new ConsulProviderOptions(),
                configuration1 => configuration1.Address = new Uri(configuration["CONSUL_URL"])));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
