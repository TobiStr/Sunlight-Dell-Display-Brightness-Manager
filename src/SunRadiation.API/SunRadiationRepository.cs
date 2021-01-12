using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SunRadiation.API
{
    public class SunRadiationRepository : ISunRadiationRepository
    {
        private readonly string forecastUrl;

        private ForecastRepository forecastRepository;

        public SunRadiationRepository(string forecastUrl) {
            if (string.IsNullOrEmpty(forecastUrl)) {
                throw new ArgumentException($"'{nameof(forecastUrl)}' cannot be null or empty", nameof(forecastUrl));
            }

            this.forecastUrl = forecastUrl;
            this.forecastRepository = LoadForecastRepository();
        }

        private ForecastRepository LoadForecastRepository() {
            var forecastFilePath = System.IO.Path.GetDirectoryName(typeof(SunRadiationRepository).Assembly.Location)
                + "\\forecast.json";

            return JsonConvert.DeserializeObject<ForecastRepository>(File.ReadAllText(forecastFilePath));
        }

        private async Task FetchNewForecast() {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(forecastUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            forecastRepository = JsonConvert.DeserializeObject<ForecastRepository>(responseBody);

            var forecastFilePath = System.IO.Path.GetDirectoryName(typeof(SunRadiationRepository).Assembly.Location)
                + "\\forecast.json";
            File.WriteAllText(forecastFilePath, responseBody);
        }

        public async Task<int> GetCurrentRadiation() {
            var now = RoundUp(DateTime.Now, TimeSpan.FromMinutes(30));
            var forecastEntry = forecastRepository.forecasts
                .FirstOrDefault(f =>
                    f.period_end.Year == now.Year
                    && f.period_end.Month == now.Month
                    && f.period_end.Day == now.Day
                    && f.period_end.Hour == now.Hour
                    && f.period_end.Minute == now.Minute
                );

            if (forecastEntry is null) {
                await FetchNewForecast();
                return await GetCurrentRadiation();
            }

            return forecastEntry.ghi;
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d) {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
    }
}