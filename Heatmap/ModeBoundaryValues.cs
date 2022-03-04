using Newtonsoft.Json;

namespace Heatmap
{
    /// <summary>
    /// Stores minimum and maximum values for every mode
    /// </summary>
    public class ModeBoundaryValues
    {
        public Boundaries TimeSpent { get; internal set; } = new Boundaries(0, 30);

        public Boundaries AverageVelocity { get; internal set; } = new Boundaries(0, 80);

        public Boundaries MaximumVelocity { get; internal set; } = new Boundaries(0, 80);

        public Boundaries Visits { get; internal set; } = new Boundaries(0, 5);

        public Boundaries TotalVisits { get; internal set; } = new Boundaries(0, 5);

        public Boundaries NodeLength { get; internal set; } = new Boundaries(0, 1);

        /// <summary>
        /// Get the minimum and maximum of the current mode
        /// </summary>
        public Boundaries GetCurrentBoundaries()
        {
            switch (Overlay.GetCurrentMode())
            {
                case OverlayMode.TimeSpent:
                    return TimeSpent;
                case OverlayMode.AverageVelocity:
                    return AverageVelocity;
                case OverlayMode.MaximumVelocity:
                    return MaximumVelocity;
                case OverlayMode.NumberOfVisits:
                    return Visits;
                case OverlayMode.TotalVisits:
                    return TotalVisits;
                case OverlayMode.NodeLength:
                    return NodeLength;
                default:
                    return new Boundaries(0, 1);
            }
        }

        /// <summary>
        /// Set the minimum of the current mode
        /// </summary>
        public void SetCurrentMinimum(int min)
        {
            switch (Overlay.GetCurrentMode())
            {
                case OverlayMode.TimeSpent:
                    TimeSpent.Minimum = min;
                    break;
                case OverlayMode.AverageVelocity:
                    AverageVelocity.Minimum = min;
                    break;
                case OverlayMode.MaximumVelocity:
                    MaximumVelocity.Minimum = min;
                    break;
                case OverlayMode.NumberOfVisits:
                    Visits.Minimum = min;
                    break;
                case OverlayMode.TotalVisits:
                    TotalVisits.Minimum = min;
                    break;
                case OverlayMode.NodeLength:
                    NodeLength.Minimum = min;
                    break;
            }
        }

        /// <summary>
        /// Set the maximum of the current mode
        /// </summary>
        /// <param name="max"></param>
        public void SetCurrentMaximum(int max)
        {
            switch (Overlay.GetCurrentMode())
            {
                case OverlayMode.TimeSpent:
                    TimeSpent.Maximum = max;
                    break;
                case OverlayMode.AverageVelocity:
                    AverageVelocity.Maximum = max;
                    break;
                case OverlayMode.MaximumVelocity:
                    MaximumVelocity.Maximum = max;
                    break;
                case OverlayMode.NumberOfVisits:
                    Visits.Maximum = max;
                    break;
                case OverlayMode.TotalVisits:
                    TotalVisits.Maximum = max;
                    break;
                case OverlayMode.NodeLength:
                    NodeLength.Maximum = max;
                    break;
            }
        }

        [JsonConstructor]
        public ModeBoundaryValues(Boundaries timeSpent, Boundaries averageVelocity, 
            Boundaries maximumVelocity, Boundaries visits, Boundaries totalVisits, Boundaries nodeLength)
        {
            if (timeSpent != null) TimeSpent = timeSpent;
            if (averageVelocity != null) AverageVelocity = averageVelocity;
            if (maximumVelocity != null) MaximumVelocity = maximumVelocity;
            if (visits != null) Visits = visits;
            if (totalVisits != null) TotalVisits = totalVisits;
            if (nodeLength != null) NodeLength = nodeLength;
        }

        internal ModeBoundaryValues() { }
    }
}
