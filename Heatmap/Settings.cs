using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Heatmap.UI;

namespace Heatmap
{
    public class Settings
    {
        [JsonIgnore]
        public static Settings Instance { get; private set; } = new Settings();

        public string GradientName { get; internal set; } = "cividis";

        [JsonIgnore]
        public ColorGradient Gradient => ColorGradient.Gradients[GradientName];

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

        // TODO: IO
    }
}
