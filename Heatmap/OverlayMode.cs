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
        internal static BoardNode TooltipSubject;

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
        public static Color GetNodeColor(BoardNode node, out float value)
        {
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            ColorGradient gradient = Settings.Instance.Gradient;
            NodeTimerTracker tracker = NodeTimerTracker.Instance;

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
                    value = tracker.GetAverageVelocity(node);
                    break;
                case OverlayMode.NodeLength:
                    value = node.GetNodeForVisualState().Length; // m
                    break;
                case OverlayMode.TotalVisits:
                    value = node.GetNodeForVisualState().UsageCount;
                    break;
                default:
                    Log($"Invalid mode when getting the color for node {node.name}", LogLevel.Exception);
                    value = 0;
                    return Color.magenta; // obvious 'there's a bug' color
            }

            float valueFraction = Mathf.Clamp01((value - boundaries.Minimum) / (boundaries.Maximum - boundaries.Minimum));
            return gradient.GetColor(valueFraction);
        }

        /// <summary>
        /// Return information about the node <paramref name="node"/>
        /// as a string to be displayed in a tooltip
        /// </summary>
        public static string GetTooltipInfo(BoardNode node)
        {
            TooltipSubject = node;

            GetNodeColor(node, out float value);
            string nodename = node.GetNodeForVisualState().FriendlyName;
            string mode = Settings.Instance.Mode;

            if (mode == "time spent")
                return $"<b>{nodename}</b>\n{mode}: {value:F2} minutes";
            else if (mode == "max. speed")
                return $"<b>{nodename}</b>\n{mode}: {value:F0} km/h";
            else if (mode == "avg. speed")
                return $"<b>{nodename}</b>\n{mode}: {value:F0} km/h";
            else if (mode == "node length")
                if (value < 1000)
                    return $"<b>{nodename}</b>\n{mode}: {value:F0} m";
                else
                    return $"<b>{nodename}</b>\n{mode}: {value/1000:F2} km";
            else
                return $"<b>{nodename}</b>\n{mode}: {value}";
        }

        /// <summary>
        /// Return information about the last node
        /// as a string to be displayed in a tooltip
        /// </summary>
        public static string GetTooltipInfo()
        {
            return GetTooltipInfo(TooltipSubject);
        }

        public static OverlayMode GetCurrentMode()
        {
            return Modes[Settings.Instance.Mode];
        }
    }
}
