using System.Diagnostics.CodeAnalysis;
using Game;
using HarmonyLib;
using Heatmap.UI;
using UnityEngine;
using static Heatmap.Logging.Logging;

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
            if (ToolbarButton.Instance.Value && Heatmap.Instance.AllowOverlay)
            {
                float occupiedFraction = NodeTimerTracker.Instance.GetOccupiedTimeFraction(__instance.name);
                __result = new Color(occupiedFraction, 1 - occupiedFraction, 0);

                Log($"Node {__instance.name} now has color {__result}", LogLevel.Debug);
            }
        }
    }
}
