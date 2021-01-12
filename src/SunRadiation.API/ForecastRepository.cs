using System;

namespace SunRadiation.API
{
    internal class ForecastRepository
    {
        public Forecast[] forecasts { get; set; }
    }

    internal class Forecast
    {
        public int ghi { get; set; }

        public int ghi90 { get; set; }

        public int ghi10 { get; set; }

        public int ebh { get; set; }

        public int dni { get; set; }

        public int dni10 { get; set; }

        public int dni90 { get; set; }

        public int dhi { get; set; }

        public int air_temp { get; set; }

        public int zenith { get; set; }

        public int azimuth { get; set; }

        public int cloud_opacity { get; set; }

        public DateTime period_end { get; set; }

        public string period { get; set; }
    }
}