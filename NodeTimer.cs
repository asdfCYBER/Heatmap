using System;

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

        public bool NodeIsCleared => TimeCleared != TimeSpan.Zero;

        /// <summary>
        /// Difference between the time at which the node
        /// became occupied and the time at which it was cleared
        /// </summary>
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

        public NodeTimer(TimeSpan timeOccupied, TimeSpan timeCleared)
        {
            TimeOccupied = timeOccupied;
            TimeCleared = timeCleared;
        }
    }
}
