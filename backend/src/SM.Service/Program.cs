using System.IO;
using Microsoft.AspNetCore.Hosting;
using Proto;
using Proto.Cluster;
using Proto.Cluster.Consul;
using Proto.Remote;
using SM.Service.Classes;
using SM.Xsd;
using Pattern = SM.Service.Classes.Pattern;

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
            var reader = new XsdPatternReader();
            var props = Actor.FromProducer(() => new Pattern(reader));
            
            // TODO: Register all known actors in a generic way 
            Remote.RegisterKnownKind("pattern", props);
            Remote.Start("127.0.0.1", 12001);
            Cluster.Start("PatternCluster", new ConsulProvider(new ConsulProviderOptions()));
        }
    }
}
