using System.IO;
using Microsoft.AspNetCore.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;

namespace SM.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            StartCluster();
            
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
        
        private static void StartCluster()
        {
            var props = Actor.FromFunc(ctx => Actor.Done);
            
            // TODO: Register all known actors in a generic way 
            Remote.RegisterKnownKind("Pattern", props);
            Remote.Start("127.0.0.1", 12001);
            Cluster.Start("PatternCluster", new ConsulProvider(new ConsulProviderOptions()));
        }
    }
}
