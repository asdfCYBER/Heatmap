using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.DeleteSave method so heatmap data can also be deleted
    /// </summary>
    [HarmonyPatch(typeof(StorageController), nameof(StorageController.DeleteSave))]
    internal class DeletePatch
    {
        private static void Prefix(StorageController.SaveFile saveFile)
        {
            Log($"Game {saveFile.FileName} is being deleted", LogLevel.Info);
            NodeTimerIO.Delete(saveFile.FileName);
        }
    }
}
