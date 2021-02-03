using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SunRadiation.API;

namespace Dell.BrightnessManager.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ILoggerFactory loggerFactory;

        public Worker(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<Worker>();
            this.loggerFactory = loggerFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            await Task.Yield();
            var settingsPath = System.IO.Path.GetDirectoryName(typeof(Worker).Assembly.Location)
                + "\\settings.json";
            var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
            var sunRadiationRepository = new SunRadiationRepository(settings.ForecastURL);

            var manager = new DellBrightnessManager(
                settings: settings,
                sunRadiationRepository: sunRadiationRepository,
                logger: loggerFactory.CreateLogger<DellBrightnessManager>());

            _logger.LogInformation("Brightness Manager Initialized");
        }
    }
}