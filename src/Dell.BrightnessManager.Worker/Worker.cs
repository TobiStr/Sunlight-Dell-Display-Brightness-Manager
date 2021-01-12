using System;
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

        public Worker(ILogger<Worker> logger) {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            var settingsPath = System.IO.Path.GetDirectoryName(typeof(Worker).Assembly.Location)
                + "\\settings.json";
            var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
            var sunRadiationRepository = new SunRadiationRepository(settings.ForecastURL);

            var manager = new DellBrightnessManager(settings, sunRadiationRepository);

            _logger.LogInformation("Brightness Manager Initialized");
        }
    }
}