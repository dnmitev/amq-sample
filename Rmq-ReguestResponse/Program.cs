using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Rmq_ReguestResponse
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.AddEnvironmentVariables("MQ_");
                   config.AddEnvironmentVariables("MQ:");
               })
              .ConfigureServices((hostContext, services) =>
              {
                  services
                    .AddSingleton<ILoggerFactory, LoggerFactory>()
                    .AddLogging(opt => opt.AddConsole(c =>
                    {
                        c.TimestampFormat = "[dd/MM/yyyy HH:mm:ss]";
                    }));

                  services.AddHostedService<MessageQueueService>();
              });

            await builder
              .RunConsoleAsync();
        }
    }
}
