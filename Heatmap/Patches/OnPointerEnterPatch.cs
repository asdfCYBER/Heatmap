using System.Diagnostics.CodeAnalysis;
using Game;
using HarmonyLib;
using Heatmap.UI;
using UnityEngine.EventSystems;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.BoardNode.CurrentColor property to allow overwriting BoardNode colors
    /// </summary>
    [HarmonyPatch(typeof(BoardNode), "OnPointerEnter")]
    internal class OnPointerEnterPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(BoardNode __instance, ref PointerEventData eventData)
        {
            if (ToolbarButton.Value && Heatmap.Instance.AllowOverlay)
            {
                Log($"OnPointerEnter node {__instance.name}", LogLevel.Debug);
                Tooltip.Show();
                Tooltip.UpdateText(Overlay.GetTooltipInfo(__instance));
            }
        }
    }
}
