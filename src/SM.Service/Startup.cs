using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SM.Service
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseDefaultFiles().UseStaticFiles();

            app.Use(async (context, next) =>
            {
                context.Response.Clear();

                // TODO: Introduce better solution to always return index.html or another default html for route.
                await context.Response.WriteAsync(File.ReadAllText("wwwroot/index.html"));
            });
        }
    }
}