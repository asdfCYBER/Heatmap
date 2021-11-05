using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.Save method so heatmap data can also be serialized
    /// </summary>
    [HarmonyPatch(typeof(StorageController), "Save")] // maybe transpiling is better to make heatmap saving async too
    internal class SavePatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(string saveName)
        {
            Log($"Game is being saved as {saveName}", LogLevel.Debug);
            NodeTimerIO.Save(saveName, NodeTimerTracker.Instance);
        }
    }
}
