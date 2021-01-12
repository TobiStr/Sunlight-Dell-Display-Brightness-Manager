using System.Collections.Generic;
using System.Linq;

namespace Dell.BrightnessManager
{
    public static class SettingsExtension
    {
        public static Dictionary<int, int> GetBrightnessDictionary(this Settings settings) {
            var s = settings;
            return new (int radiation, int brightness)[] {
                (s.SolarLevel1, s.BrightnessLevel1),
                (s.SolarLevel2, s.BrightnessLevel2),
                (s.SolarLevel3, s.BrightnessLevel3),
                (s.SolarLevel4, s.BrightnessLevel4),
                (s.SolarLevel5, s.BrightnessLevel5),
                (s.SolarLevel6, s.BrightnessLevel6),
                (s.SolarLevel7, s.BrightnessLevel7),
                (s.SolarLevel8, s.BrightnessLevel8),
                (s.SolarLevel9, s.BrightnessLevel9),
            }.ToDictionary(x => x.radiation, x => x.brightness);
        }
    }
}