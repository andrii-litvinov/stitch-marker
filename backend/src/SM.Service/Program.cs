using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

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
                .Build()
                .Run();
        }
    }
}
