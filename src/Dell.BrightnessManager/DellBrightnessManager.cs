using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using SunRadiation.API;

namespace Dell.BrightnessManager
{
    public class DellBrightnessManager
    {
        private readonly Settings settings;

        private readonly ISunRadiationRepository sunRadiationRepository;

        private readonly Timer timer;

        public DellBrightnessManager(Settings settings, ISunRadiationRepository sunRadiationRepository) {
            this.settings = settings
                ?? throw new ArgumentNullException(nameof(settings));
            this.sunRadiationRepository = sunRadiationRepository
                ?? throw new ArgumentNullException(nameof(sunRadiationRepository));
            this.timer = new Timer();
            StartTimer();
        }

        private void StartTimer() {
            timer.Interval = 600000;
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e) {
            RefreshBrightness(settings, sunRadiationRepository);
        }

        private static async Task RefreshBrightness(Settings settings, ISunRadiationRepository sunRadiationRepository) {
            var brightnessDictionary = settings.GetBrightnessDictionary();
            var currentRadiation = await sunRadiationRepository.GetCurrentRadiation();

            var brightness = brightnessDictionary.OrderBy(kvp => currentRadiation - kvp.Key).Last().Value;
            SetBrightness(brightness, settings.ddmExecutablePath);
        }

        private static void SetBrightness(int brightness, string executablePath) {
            ProcessStartInfo processInfo;
            Process process;
            processInfo = new ProcessStartInfo(executablePath, $"/SetBrightnessLevel {brightness}") {
                UseShellExecute = false
            };

            process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }
    }
}