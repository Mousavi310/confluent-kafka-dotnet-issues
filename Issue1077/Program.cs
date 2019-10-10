using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Issue1077
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) => 
                {

                })
                .ConfigureServices((hostContext, services) => 
                {
                    services.AddHostedService<KafkaHostedService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                });
            
            await builder.RunConsoleAsync();
        }
    }
}
