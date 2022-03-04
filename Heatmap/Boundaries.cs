using Newtonsoft.Json;

namespace Heatmap
{
    public class Boundaries
    {
        public int Minimum { get; internal set; }

        public int Maximum { get; internal set; }

        [JsonConstructor]
        public Boundaries(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}
