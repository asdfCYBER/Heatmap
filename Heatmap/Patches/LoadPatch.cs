using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.Load method so saved heatmap data can also be deserialized
    /// </summary>
    [HarmonyPatch(typeof(StorageController), nameof(StorageController.Load))]
    internal class LoadPatch
    {
        private static void Prefix(StorageController.SaveFile saveFile)
        {
            Log($"Game {saveFile.FileName} is being loaded", LogLevel.Info);
            NodeTimerIO.Load(saveFile.FileName, NodeTimerTracker.Instance);
        }
    }
}
