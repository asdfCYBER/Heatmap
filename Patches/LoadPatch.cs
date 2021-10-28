using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.Load method so saved heatmap data can also be deserialized
    /// </summary>
    [HarmonyPatch(typeof(StorageController), "Load")]
    internal class LoadPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(string saveName)
        {
            Log($"Game {saveName} is being loaded", LogLevel.Debug);
            NodeTimerIO.Load(saveName, NodeTimerTracker.Instance);
        }
    }
}
