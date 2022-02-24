using System.Collections.Generic;
using Game;
using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public enum OverlayMode
    {
        None,
        TimeSpent,
        NumberOfVisits,
        MaximumVelocity,
        AverageVelocity,
        NodeLength,
        TotalVisits
    }

    public static class Overlay
    {
        /// <summary>
        /// Link between dropdown options and their enum representation
        /// </summary>
        public static Dictionary<string, OverlayMode> Modes { get; } = new Dictionary<string, OverlayMode>()
        {
            { "time spent", OverlayMode.TimeSpent },
            { "visits", OverlayMode.NumberOfVisits },
            { "max. speed", OverlayMode.MaximumVelocity },
            { "avg. speed", OverlayMode.AverageVelocity },
            { "node length", OverlayMode.NodeLength },
            { "total visits", OverlayMode.TotalVisits }
        };

        /// <summary>
        /// Get the color for the node, taking the current mode into account
        /// </summary>
        public static Color GetNodeColor(BoardNode node)
        {
            int min = Settings.Instance.BusynessMinimum;
            int max = Settings.Instance.BusynessMaximum;
            ColorGradient gradient = Settings.Instance.Gradient;
            NodeTimerTracker tracker = NodeTimerTracker.Instance;

            float value;
            switch (Modes[Settings.Instance.Mode])
            {
                case OverlayMode.TimeSpent:
                    value = tracker.GetOccupiedTimeMinutes(node.name);
                    break;
                case OverlayMode.MaximumVelocity:
                    value = node.GetNodeForVisualState().MaxSpeed * 3.6f; // m/s to km/h
                    break;
                case OverlayMode.NumberOfVisits:
                    value = tracker.GetNodeTimerCount(node.name);
                    break;
                case OverlayMode.AverageVelocity:
                    float length = node.GetNodeForVisualState().Length / 1000f; // km
                    float averageTime = tracker.GetAverageTimeMinutes(node.name) / 60f; // hour
                    value = GetAverageVelocity(length, averageTime);
                    break;
                case OverlayMode.NodeLength:
                    value = node.GetNodeForVisualState().Length / 1000f; // km
                    break;
                case OverlayMode.TotalVisits:
                    value = node.GetNodeForVisualState().UsageCount;
                    break;
                default:
                    Log($"Invalid mode when getting the color for node {node.name}", LogLevel.Exception);
                    return Color.magenta; // obvious 'there's a bug' color
            }

            float valueFraction = Mathf.Clamp01((value - min) / max);
            return gradient.GetColor(100 * valueFraction);
        }

        public static float GetAverageVelocity(float length, float time)
        {
            if (time == 0)
                return 0;

            return length / time;
        }
    }
}
