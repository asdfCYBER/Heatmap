using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.Save method so heatmap data can also be serialized
    /// </summary>
    [HarmonyPatch(typeof(StorageController), nameof(StorageController.Save))]
    internal class SavePatch
    {
        private static void Prefix(string saveName)
        {
            Log($"Game is being saved as {saveName}", LogLevel.Debug);
            NodeTimerIO.Save(saveName, NodeTimerTracker.Instance);
        }
    }
}
