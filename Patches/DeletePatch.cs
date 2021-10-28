using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.DeleteSave method so heatmap data can also be deleted
    /// </summary>
    [HarmonyPatch(typeof(StorageController), "DeleteSave")]
    internal class DeletePatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(string saveName)
        {
            Log($"Game {saveName} is being deleted", LogLevel.Debug);
            NodeTimerIO.Delete(saveName);
        }
    }
}
