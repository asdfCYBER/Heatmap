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
    [HarmonyPatch(typeof(BoardNode), "OnPointerExit")]
    internal class OnPointerExitPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(BoardNode __instance)
        {
            if (ToolbarButton.Value && Heatmap.Instance.AllowOverlay)
                Tooltip.Hide();
        }
    }
}
