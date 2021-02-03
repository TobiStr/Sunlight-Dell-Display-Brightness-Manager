using System;
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
                .Build().StartAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();
                }).ConfigureLogging((hostingContext, builder) => {
                    builder.AddFile($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}"
                + "\\DellBrightnessManager\\Logs\\log_{Date}.log", LogLevel.Information);
                })
            .UseWindowsService();
    }
}