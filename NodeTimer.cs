using System;
using Newtonsoft.Json;

namespace Heatmap
{
    /// <summary>
    /// Stores information about how long a node is occupied
    /// </summary>
    public class NodeTimer
    {
        /// <summary>
        /// Time at which the node became occupied
        /// </summary>
        public TimeSpan TimeOccupied { get; set; }

        /// <summary>
        /// Time at which the last train left the node
        /// </summary>
        public TimeSpan TimeCleared { get; set; }

        [JsonIgnore]
        public bool NodeIsCleared => TimeCleared != TimeSpan.Zero;

        /// <summary>
        /// Difference between the time at which the node
        /// became occupied and the time at which it was cleared
        /// </summary>
        [JsonIgnore]
        public TimeSpan TimeElapsed
        {
            get
            {
                if (NodeIsCleared)
                    return TimeCleared - TimeOccupied;
                else
                    return NodeTimerTracker.CurrentTime - TimeOccupied;
            }
        }

        public override string ToString()
        {
            return $"NodeTimer instance which started at {TimeOccupied} and " +
                $"ended at {TimeCleared} (total time elapsed: {TimeElapsed})";
        }

        public NodeTimer(TimeSpan timeOccupied)
        {
            TimeOccupied = timeOccupied;
            TimeCleared = TimeSpan.Zero;
        }

        [JsonConstructor]
        public NodeTimer(TimeSpan timeOccupied, TimeSpan timeCleared)
        {
            TimeOccupied = timeOccupied;
            TimeCleared = timeCleared;
        }
    }
}
