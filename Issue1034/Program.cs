using System;
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
                    //services.AddHostedService<>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConsole();
                });
        }
    }
}
