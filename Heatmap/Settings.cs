using Newtonsoft.Json;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public class Settings
    {
        [JsonIgnore]
        public static Settings Instance { get; private set; } = new Settings();

        /// <summary>
        /// The name of the <see cref="ColorGradient"/> used for the heatmap
        /// </summary>
        public string GradientName { get; internal set; } = "cividis";

        /// <summary>
        /// The <see cref="ColorGradient"/> used for the heatmap
        /// </summary>
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

        // TODO: add warning panel to confirm lower changes (and maybe settings in general?)
        /// <summary>
        /// The period (in minutes) used for calculating the heatmap data
        /// </summary>
        public int MeasuringPeriod { get; internal set; } = 30;

        /// <summary>
        /// The amount of minutes that count as 0% in a gradient
        /// </summary>
        public int BusynessMinimum { get; internal set; } = 0;

        /// <summary>
        /// The amount of minutes that count as 100% in a gradient
        /// </summary>
        public int BusynessMaximum { get; internal set; } = 30;

        [JsonConstructor]
        internal Settings(string gradientName, int? measuringPeriod,
            int? busynessMinimum, int? busynessMaximum)
        {
            Instance = this;
            if (!string.IsNullOrWhiteSpace(gradientName)) GradientName = gradientName;
            if (measuringPeriod.HasValue) MeasuringPeriod = measuringPeriod.Value;
            if (busynessMinimum.HasValue) BusynessMinimum = busynessMinimum.Value;
            if (busynessMaximum.HasValue) BusynessMaximum = busynessMaximum.Value;
        }
        
        private Settings() { }
    }
}
