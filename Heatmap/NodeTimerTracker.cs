using System;
using System.Collections.Generic;
using System.Linq;
using Game.Railroad;
using Game;
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

        public static TimeSpan CurrentTime => Heatmap._timeController.CurrentTime;

        /// <summary>
        /// Registers the node in <see cref="NodeTimers"/> as occupied
        /// </summary>
        /// <param name="node">The node which has been entered</param>
        /// <param name="previouslyUnregisted">Whether a train was detected as entering</param>
        public void RegisterNodeEntered(Node node, bool previouslyUnregistered = false, int trainLength = 0)
        {
            if (!NodeTimers.ContainsKey(node.Name))
            {
                NodeTimers[node.Name] = new List<NodeTimer>();
            }

            // if there is a NodeTime which has not been closed, don't add a new NodeTime
            if (!NodeTimers[node.Name].Any(timer => !timer.NodeIsCleared))
            {
                NodeTimers[node.Name].Add(new NodeTimer(CurrentTime, trainLength));

                if (previouslyUnregistered)
                    Log($"Found unregistered train on node {node.FriendlyName}, " +
                        "the node is now occupied", LogLevel.Warning);
                else
                    Log($"Registered: node {node.FriendlyName} is now occupied", LogLevel.Debug);
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
                    $"The node was occupied for {nodeTimer.TimeElapsed}", LogLevel.Debug);
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
                    RegisterNodeEntered(node, previouslyUnregistered: true);
                }
            }
        }

        /// <summary>
        /// Return the amount of time the node with 
        /// name <paramref name="nodename"/> was occupied in ticks
        /// </summary>
        public long GetOccupiedTimeTicks(string nodename)
        {
            if (!NodeTimers.ContainsKey(nodename))
                return 0;

            // use data from the last MeasuringPeriod minutes
            TimeSpan deletePeriodEnd = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.DeleteAfter);
            TimeSpan measuringTimeStart = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.MeasuringPeriod);
            HashSet<NodeTimer> deletableTimers = new HashSet<NodeTimer>();

            bool nodetimersDeleted = false;

            // The occupied time in milliseconds
            long occupiedTime = 0;
            foreach (NodeTimer nodeTimer in NodeTimers[nodename])
            {
                // if TimeOccupied was later than the start of the measuring
                // period, the entire occupied time is measured
                if (nodeTimer.TimeOccupied >= measuringTimeStart)
                {
                    occupiedTime += nodeTimer.TimeElapsed.Ticks;
                }
                // if TimeOccupied was before the start of the measuring period
                // but TimeCleared after, the occupied time is partially counted
                else if (!nodeTimer.NodeIsCleared || nodeTimer.TimeCleared > measuringTimeStart)
                {
                    // amount of time which falls outside of the measuring period
                    TimeSpan notmeasuredTime = measuringTimeStart - nodeTimer.TimeOccupied;
                    occupiedTime += (nodeTimer.TimeElapsed - notmeasuredTime).Ticks;
                }
                // if both TimeOccupied and TimeCleared occured before the measuring period started
                // and TimeOccupied is older than the deletion threshold then the timer can be deleted
                else if (nodeTimer.TimeOccupied < deletePeriodEnd
                    && nodeTimer.NodeIsCleared && nodeTimer.TimeCleared < measuringTimeStart)
                {
                    deletableTimers.Add(nodeTimer);
                    nodetimersDeleted = true;
                }
                // otherwise, nodeTimer.TimeCleared is between deletePeriodEnd
                // and measuringTimeStart and nothing needs to happen
            }

            if (nodetimersDeleted)
                NodeTimers[nodename].RemoveAll(timer => deletableTimers.Contains(timer));

            return occupiedTime;
        }

        /// <summary>
        /// Return the amount of time the node with 
        /// name <paramref name="nodename"/> was occupied in minutes
        /// </summary>
        public float GetOccupiedTimeMinutes(string nodename)
        {
            // ticks to minutes: 10 million ticks in a second, 60 seconds in a minute
            return GetOccupiedTimeTicks(nodename) / 6e8f;
        }

        /// <summary>
        /// Return the busyness as a value between 0 and 1 where 0 is 
        /// any value <= <see cref="Settings.BusynessMinimum"/> and 
        /// 1 any value >= <see cref="Settings.BusynessMaximum"/>
        /// </summary>
        public float GetBusynessFraction(string nodename)
        {
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            float minutes = GetOccupiedTimeMinutes(nodename);
            float fraction;

            if (minutes < boundaries.Minimum) 
                fraction = 0;
            else if (minutes > boundaries.Maximum) 
                fraction = 1;
            else
            {
                fraction = minutes - boundaries.Minimum;
                fraction /= (boundaries.Maximum - boundaries.Minimum);
            }

            Log($"Node {nodename} has a busyness fraction of {fraction:P1}", LogLevel.Debug);
            return fraction;
        }

        /// <summary>
        /// Return the amount of valid timers on this node
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public int GetNodeTimerCount(string nodename)
        {
            if (!NodeTimers.ContainsKey(nodename))
                return 0;

            // Use data from the last MeasuringPeriod minutes
            // using NodeTimers[nodename].Count() is insufficient because expired timers have to be deleted
            TimeSpan deletePeriodEnd = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.DeleteAfter);
            TimeSpan measuringTimeStart = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.MeasuringPeriod);
            HashSet<NodeTimer> deletableTimers = new HashSet<NodeTimer>();

            bool nodetimersDeleted = false;

            // The amount of timers
            int count = 0;
            foreach (NodeTimer nodeTimer in NodeTimers[nodename])
            {
                // All timers that are still going or were cleared in the measuring range count
                if (!nodeTimer.NodeIsCleared || nodeTimer.TimeCleared > measuringTimeStart)
                {
                    count++;
                }
                // If both TimeOccupied and TimeCleared occured before the measuring period started
                // and TimeOccupied is older than the deletion threshold then the timer can be deleted
                else if (nodeTimer.TimeOccupied < deletePeriodEnd
                    && nodeTimer.NodeIsCleared && nodeTimer.TimeCleared < measuringTimeStart)
                {
                    deletableTimers.Add(nodeTimer);
                    nodetimersDeleted = true;
                }
                // Otherwise, nodeTimer.TimeCleared is between deletePeriodEnd
                // and measuringTimeStart and nothing needs to happen
            }

            if (nodetimersDeleted)
                NodeTimers[nodename].RemoveAll(timer => deletableTimers.Contains(timer));

            return count;
        }

        /// <summary>
        /// Get the average velocity trains travel at in a node by dividing the node length plus 
        /// train length by the time it took that train to exit the node, then taking the average.
        /// </summary>
        /// <returns>Average velocity in km/h</returns>
        public float GetAverageVelocity(BoardNode node)
        {
            if (!NodeTimers.ContainsKey(node.name))
                return 0;

            // use data from the last MeasuringPeriod minutes
            TimeSpan deletePeriodEnd = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.DeleteAfter);
            TimeSpan measuringTimeStart = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.MeasuringPeriod);
            HashSet<NodeTimer> deletableTimers = new HashSet<NodeTimer>();

            bool nodetimersDeleted = false;

            // The occupied time in milliseconds
            int count = 0;
            float velocitySum = 0;
            float nodelength = node.Node.Length;

            foreach (NodeTimer nodeTimer in NodeTimers[node.name])
            {
                // Only count nodes that fall entirely in the measuring period
                if (nodeTimer.NodeIsCleared && nodeTimer.TimeOccupied > measuringTimeStart)
                {
                    velocitySum += (nodelength + nodeTimer.TrainLength) / (float)nodeTimer.TimeElapsed.TotalSeconds;
                    count += 1;
                }

                // If both TimeOccupied and TimeCleared occured before the measuring period started
                // and TimeOccupied is older than the deletion threshold then the timer can be deleted
                else if (nodeTimer.TimeOccupied < deletePeriodEnd
                    && nodeTimer.NodeIsCleared && nodeTimer.TimeCleared < measuringTimeStart)
                {
                    deletableTimers.Add(nodeTimer);
                    nodetimersDeleted = true;
                }
            }

            if (nodetimersDeleted)
                NodeTimers[node.name].RemoveAll(timer => deletableTimers.Contains(timer));

            if (count == 0)
                return 0;
            else
                return velocitySum / count * 3.6f; // average velocity, convert from m/s to km/h
        }

        // Hide the constructor, this class should not be instantiated twice
        private NodeTimerTracker() { }
    }
}
