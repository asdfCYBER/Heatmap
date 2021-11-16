using Newtonsoft.Json;

namespace Heatmap
{
    public class Settings
    {
        [JsonIgnore]
        public static Settings Instance { get; private set; } = new Settings();

        public string GradientName { get; internal set; } = "cividis";

        [JsonIgnore]
        public ColorGradient Gradient
        {
            get
            {
                if (ColorGradient.Gradients.ContainsKey(GradientName))
                    return ColorGradient.Gradients[GradientName];
                else
                {
                    // Loaded settings were nonsense, probably
                    GradientName = "cividis";
                    return ColorGradient.Cividis;
                }
            }
        }

        public int MeasuringPeriod { get; internal set; } = 30;

        public int BusynessMultiplier { get; internal set; } = 2;

        [JsonConstructor]
        internal Settings(string gradientName, int measuringPeriod, int busynessMultiplier)
        {
            Instance = this;
            GradientName = gradientName;
            MeasuringPeriod = measuringPeriod;
            BusynessMultiplier = busynessMultiplier;
        }
        
        private Settings() { }
    }
}
