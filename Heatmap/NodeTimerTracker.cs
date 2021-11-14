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
        public Dictionary<string, List<NodeTimer>> NodeTimers = 
            new Dictionary<string, List<NodeTimer>>();

        public static TimeSpan CurrentTime => 
            Heatmap._controller.GameControllers.TimeController.CurrentTime;

        /// <summary>
        /// Registers the node in <see cref="NodeTimers"/> as occupied
        /// </summary>
        /// <param name="node">The node which has been entered</param>
        /// <param name="previouslyUnregisted">Whether a train was detected as entering</param>
        public void RegisterNodeEntered(Node node, bool previouslyUnregisted = false)
        {
            if (!NodeTimers.ContainsKey(node.Name))
            {
                NodeTimers[node.Name] = new List<NodeTimer>();
            }

            // if there is a NodeTime which has not been closed, don't add a new NodeTime
            if (!NodeTimers[node.Name].Any(timer => !timer.NodeIsCleared))
            {
                NodeTimers[node.Name].Add(new NodeTimer(CurrentTime));

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
            if (node.Trains.Count() != 0)
                return;

            if (!NodeTimers.ContainsKey(node.Name))
            {
                Log($"A train exited {node.FriendlyName} but the node was " +
                    $"never registered as occupied", LogLevel.Warning);
            }
            else if (!NodeTimers[node.Name].Any(timer => !timer.NodeIsCleared))
            {
                Log($"A train exited {node.FriendlyName} but the node was " +
                    $"never registered as occupied", LogLevel.Warning);
            }
            else
            {
                // mark the node as cleared
                NodeTimer nodeTimer = NodeTimers[node.Name].Single(timer => !timer.NodeIsCleared);
                nodeTimer.TimeCleared = CurrentTime;
                Log($"Registered: node {node.FriendlyName} is no longer occupied. " +
                    $"The node was occupied for {nodeTimer.TimeElapsed}", LogLevel.Info);
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

        /// <summary>
        /// Returns the amount of time the node was occupied in 
        /// a certain total amount of time by that total amount
        /// </summary>
        public float GetOccupiedTimeFraction(string nodename)
        {
            if (!NodeTimers.ContainsKey(nodename))
                return 0;

            // use data from the last MeasuringPeriod minutes
            TimeSpan measuringTime = TimeSpan.FromMinutes(Settings.Instance.MeasuringPeriod);
            TimeSpan measuringTimeStart = CurrentTime - measuringTime;
            List<NodeTimer> deletableTimers = new List<NodeTimer>();

            // The occupied time in milliseconds
            double occupiedTime = 0;
            foreach (NodeTimer nodeTimer in NodeTimers[nodename])
            {
                // if TimeOccupied was later than the start of the measuring
                // period, the entire occupied time is measured
                if (nodeTimer.TimeOccupied >= measuringTimeStart)
                {
                    occupiedTime += nodeTimer.TimeElapsed.TotalMilliseconds;
                    Log($"Adding {occupiedTime} ms for timer entirely in " +
                        $"the measuring period", LogLevel.Debug);
                }
                // if TimeOccupied was before the start of the measuring period
                // but TimeCleared after, the occupied time is partially counted
                else if (!nodeTimer.NodeIsCleared || nodeTimer.TimeCleared > measuringTimeStart)
                {
                    // amount of time which falls outside of the measuring period
                    TimeSpan notmeasuredTime = measuringTimeStart - nodeTimer.TimeOccupied;
                    occupiedTime += (nodeTimer.TimeElapsed - notmeasuredTime).TotalMilliseconds;
                    Log($"Adding {occupiedTime} ms for timer partially in " +
                        $"the measuring period", LogLevel.Debug);
                }
                // if both TimeOccupied and TimeCleared occured before the
                // measuring period started then the NodeTimer can be deleted
                else
                {
                    Log($"Marked {nodeTimer} as deletable at time {CurrentTime}", LogLevel.Debug);
                    deletableTimers.Add(nodeTimer);
                }
            }

            NodeTimers[nodename].RemoveAll(timer => deletableTimers.Contains(timer));

            double fraction = occupiedTime / measuringTime.TotalMilliseconds;
            Log($"Node {nodename} has a busyness factor of {fraction:P1}", LogLevel.Debug);
            return (float)fraction;
        }

        // Hide the constructor, this class should not be instantiated twice
        private NodeTimerTracker() { }
    }
}
