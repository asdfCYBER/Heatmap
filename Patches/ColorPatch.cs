using System.Diagnostics.CodeAnalysis;
using Game;
using HarmonyLib;
using static Heatmap.Logging.Logging;
using Heatmap.UI;
using UnityEngine;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.BoardNode.CurrentColor property to allow overwriting BoardNode colors
    /// </summary>
    [HarmonyPatch(typeof(BoardNode), "get_CurrentColor")]
    internal class ColorPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Postfix(BoardNode __instance, ref Color __result)
        {
            if (HeatmapToggle.HeatmapEnabled && Heatmap.Instance.AllowOverlay)
            {
                // TODO: replace this proof of concept with NodeTimeTracker stuff
                float usagecount = __instance.GetNodeForVisualState().UsageCount;
                usagecount /= 5;
                __result = new Color(usagecount, 1 - usagecount, 0);

                Log($"Node {__instance.name} now has color {__result}", LogLevel.Debug);
            }
        }
    }
}
