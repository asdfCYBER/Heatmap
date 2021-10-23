using System;
using System.Collections.Generic;
using System.Linq;
using Game.Railroad;
using Game.Train;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public class NodeTimerTracker
    {
        public static NodeTimerTracker Instance { get; } = new NodeTimerTracker();

        /// <summary>
        /// Contains information about when nodes were occupied
        /// </summary>
        public Dictionary<Node, ICollection<NodeTimer>> NodeTimers = 
            new Dictionary<Node, ICollection<NodeTimer>>();

        public static TimeSpan CurrentTime => 
            Heatmap._controller.GameControllers.TimeController.CurrentTime;

        /// <summary>
        /// Registers the node in <see cref="NodeTimers"/> as occupied
        /// </summary>
        /// <param name="node">The node which has been entered</param>
        /// <param name="previouslyUnregisted">Whether a train was detected as entering</param>
        public void RegisterNodeEntered(Node node, bool previouslyUnregisted = false)
        {
            if (!NodeTimers.ContainsKey(node))
            {
                NodeTimers[node] = new List<NodeTimer>();
            }

            // if there is a NodeTime which has not been closed, don't add a new NodeTime
            if (!NodeTimers[node].Any(timer => !timer.NodeIsCleared))
            {
                NodeTimers[node].Add(new NodeTimer(CurrentTime));

                if (previouslyUnregisted)
                    Log($"Found unregistered train on node {node.FriendlyName}, " +
                        "the node is now occupied", LogLevel.Warning);
                else
                    Log($"Registered: node {node.FriendlyName} is now occupied", LogLevel.Info);
            }
        }

        /// <summary>
        /// Registers the node in <see cref="NodeTimers"/> as no longer occupied
        /// </summary>
        /// <param name="node">The node which has been exited</param>
        public void RegisterNodeExited(Node node)
        {
            // check if there are no trains in the node, don't set a TimeCleared if it is not
            if (node.Trains.Count() == 0)
            {
                if (!NodeTimers.ContainsKey(node) || 
                    !NodeTimers[node].Any(timer => !timer.NodeIsCleared))
                {
                    Log($"A train exited {node.FriendlyName} but the node was " +
                        $"never registered as occupied", LogLevel.Warning);
                }
                else
                {
                    // if there is a NodeTime which has not been cleared, close it
                    NodeTimer nodeTimer = NodeTimers[node].Single(timer => !timer.NodeIsCleared);
                    nodeTimer.TimeCleared = CurrentTime;
                    Log($"Registered: node {node.FriendlyName} is no longer occupied. " +
                        $"The node was occupied for {nodeTimer.TimeElapsed}", LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Registers already occupied nodes in NodeTimerTracker
        /// </summary>
        public void RegisterExistingTrains()
        {
            foreach (Train train in Heatmap._controller.TrainRepository.Trains)
            {
                foreach (Node node in train.OccupiedNodes)
                {
                    RegisterNodeEntered(node, true);
                }
            }
        }

        // Hide the constructor
        private NodeTimerTracker() { }
    }
}
