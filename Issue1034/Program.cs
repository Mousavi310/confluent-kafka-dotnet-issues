using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Issue1034
{
    class Program
    {
        static void Main(string[] args)
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
        }
    }
}
