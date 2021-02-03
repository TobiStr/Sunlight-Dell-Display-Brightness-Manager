using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Logging;
using SunRadiation.API;

namespace Dell.BrightnessManager
{
    public class DellBrightnessManager
    {
        private readonly Settings settings;

        private readonly ISunRadiationRepository sunRadiationRepository;

        private readonly Timer timer;

        private readonly ILogger<DellBrightnessManager> logger;

        public DellBrightnessManager(Settings settings, ISunRadiationRepository sunRadiationRepository, ILogger<DellBrightnessManager> logger)
        {
            this.settings = settings
                ?? throw new ArgumentNullException(nameof(settings));
            this.sunRadiationRepository = sunRadiationRepository
                ?? throw new ArgumentNullException(nameof(sunRadiationRepository));
            this.timer = new Timer();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            StartTimer();
        }

        private void StartTimer() {
            timer.Interval = settings.RefreshIntervallMs;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e) {
            await RefreshBrightness(settings, sunRadiationRepository);
        }

        private async Task RefreshBrightness(Settings settings, ISunRadiationRepository sunRadiationRepository) {
            logger.LogInformation("Refreshing Brightness...");

            var brightnessDictionary = settings.GetBrightnessDictionary();

            logger.LogInformation("Getting Current estimated radiation...");
            var currentRadiation = await sunRadiationRepository.GetCurrentRadiation();

            logger.LogInformation($"Current estimated radiation is {currentRadiation}.");

            var brightness = brightnessDictionary.OrderBy(kvp => currentRadiation - kvp.Key).Last().Value;

            logger.LogInformation($"Configured Brightness for radiation {currentRadiation} is {brightness}.");

            SetBrightness(brightness, settings.ddmExecutablePath);
        }

        private void SetBrightness(int brightness, string executablePath) {
            ProcessStartInfo processInfo;
            Process process;
            
            logger.LogInformation($"Setting Brightness to {brightness}...");

            processInfo = new ProcessStartInfo(executablePath, $"/SetBrightnessLevel {brightness}") {
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();

            logger.LogInformation($"Brightness set successfully.");
        }
    }
}