using System.Diagnostics.CodeAnalysis;
using Game.Hud.InGame.ConfigurationPanel.Variants;
using HarmonyLib;
using Heatmap.UI;

namespace Heatmap.Patches
{
    [HarmonyPatch(typeof(InterfaceConfigurationPanelView), "UpdateSettingsVisibility")]
    internal static class AddButtonPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051", Justification = "Used by Harmony")]
        private static void Postfix(InterfaceConfigurationPanelView __instance)
        {
            ToolbarButton.Create(__instance);
        }
    }
}
