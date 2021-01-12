using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dell.BrightnessManager.Worker
{
    public class Program
    {
        public static async Task Main(string[] args) {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = CreateHostBuilder(args)
                .UseEnvironment(isService ? Environments.Production : Environments.Development);

            if (isService) {
                await builder.RunAsServiceAsync();
            } else {
                await builder.RunConsoleAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => {
                    services.AddHostedService<Worker>();
                }).ConfigureLogging((hostingContext, logging) => {
                    logging.AddFile(config => {
                        config.Extension = ".log";
                    });
                });
    }
}