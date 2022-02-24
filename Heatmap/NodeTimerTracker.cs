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

        public static TimeSpan CurrentTime => Heatmap._timeController.CurrentTime;

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
                    RegisterNodeEntered(node, true);
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
                else if (nodeTimer.TimeOccupied < deletePeriodEnd)
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
            float minutes = GetOccupiedTimeMinutes(nodename);
            float fraction;

            if (minutes < Settings.Instance.BusynessMinimum) 
                fraction = 0;
            else if (minutes > Settings.Instance.BusynessMaximum) 
                fraction = 1;
            else
            {
                fraction = minutes - Settings.Instance.BusynessMinimum;
                fraction /= (Settings.Instance.BusynessMaximum - Settings.Instance.BusynessMinimum);
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
                else if (nodeTimer.TimeOccupied < deletePeriodEnd)
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
        /// Get the average time spent in the node by dividing the total time by the amount of times.
        /// More efficient than calling both GetOccupiedTimeTicks and GetNodeTimerCount
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public float GetAverageTimeTicks(string nodename)
        {
            if (!NodeTimers.ContainsKey(nodename))
                return 0;

            // use data from the last MeasuringPeriod minutes
            TimeSpan deletePeriodEnd = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.DeleteAfter);
            TimeSpan measuringTimeStart = CurrentTime - TimeSpan.FromMinutes(Settings.Instance.MeasuringPeriod);
            HashSet<NodeTimer> deletableTimers = new HashSet<NodeTimer>();

            bool nodetimersDeleted = false;

            // The occupied time in milliseconds
            int count = 0;
            long occupiedTime = 0;
            foreach (NodeTimer nodeTimer in NodeTimers[nodename])
            {
                // if TimeOccupied was later than the start of the measuring
                // period, the entire occupied time is measured
                if (nodeTimer.TimeOccupied >= measuringTimeStart)
                {
                    count++;
                    occupiedTime += nodeTimer.TimeElapsed.Ticks;
                }
                // if TimeOccupied was before the start of the measuring period
                // but TimeCleared after, the occupied time is partially counted
                else if (!nodeTimer.NodeIsCleared || nodeTimer.TimeCleared > measuringTimeStart)
                {
                    count++;

                    // amount of time which falls outside of the measuring period
                    TimeSpan notmeasuredTime = measuringTimeStart - nodeTimer.TimeOccupied;
                    occupiedTime += (nodeTimer.TimeElapsed - notmeasuredTime).Ticks;
                }
                // if both TimeOccupied and TimeCleared occured before the measuring period started
                // and TimeOccupied is older than the deletion threshold then the timer can be deleted
                else if (nodeTimer.TimeOccupied < deletePeriodEnd)
                {
                    deletableTimers.Add(nodeTimer);
                    nodetimersDeleted = true;
                }
                // otherwise, nodeTimer.TimeCleared is between deletePeriodEnd
                // and measuringTimeStart and nothing needs to happen
            }

            if (nodetimersDeleted)
                NodeTimers[nodename].RemoveAll(timer => deletableTimers.Contains(timer));

            return (float)occupiedTime / count;
        }

        public float GetAverageTimeMinutes(string nodename)
        {
            // ticks to minutes: 10 million ticks in a second, 60 seconds in a minute
            return GetAverageTimeTicks(nodename) / 6e8f;
        }

        // Hide the constructor, this class should not be instantiated twice
        private NodeTimerTracker() { }
    }
}
