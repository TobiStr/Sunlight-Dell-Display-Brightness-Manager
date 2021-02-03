using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dell.BrightnessManager.Worker
{
    public class Program
    {
        public static void Main(string[] args) {
             CreateHostBuilder(args)
                .Build().Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();
                }).ConfigureLogging((hostingContext, logging) => {
                    logging.AddFile(config => {
                        config.Extension = ".log";
                    });
                })
            .UseWindowsService();
    }
}