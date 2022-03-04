using System.Collections.Generic;
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

        /// <summary>
        /// The period (in minutes) used for calculating the heatmap data
        /// </summary>
        public int MeasuringPeriod { get; internal set; } = 30;

        /// <summary>
        /// Minimum and maximum values for each mode
        /// </summary>
        public ModeBoundaryValues BoundaryValues { get; internal set; } = new ModeBoundaryValues();

        /// <summary>
        /// The amount of minutes after which node data is deleted
        /// </summary>
        public int DeleteAfter { get; internal set; } = 30;

        /// <summary>
        /// How the tracks are colored
        /// </summary>
        public string Mode { get; internal set; } = "time spent";


        [JsonConstructor]
        internal Settings(string gradientName, int? measuringPeriod,
            ModeBoundaryValues boundaryValues, int? deleteAfter, string mode)
        {
            Instance = this;
            if (!string.IsNullOrWhiteSpace(gradientName)) GradientName = gradientName;
            if (measuringPeriod.HasValue) MeasuringPeriod = measuringPeriod.Value;
            if (boundaryValues != null) BoundaryValues = boundaryValues;
            if (deleteAfter.HasValue) DeleteAfter = deleteAfter.Value;
            if (!string.IsNullOrWhiteSpace(mode)) Mode = mode;
        }
        
        private Settings() { }
    }
}
