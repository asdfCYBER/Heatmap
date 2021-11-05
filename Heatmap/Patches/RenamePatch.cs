using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Game.Level;
using Heatmap.IO;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch the Game.Level.StorageController.Rename method so saved heatmap data can also be renamed
    /// </summary>
    [HarmonyPatch(typeof(StorageController), "Rename")]
    internal class RenamePatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(StorageController.SaveFile oldSave, out StorageController.SaveFile __state)
        {
            // so the original argument can be used after the function has run
            __state = oldSave;
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Postfix(StorageController.SaveFile __result, StorageController.SaveFile __state)
        {
            Log($"Game save {__state.FileName} is being renamed to {__result.FileName}", LogLevel.Debug);
            NodeTimerIO.Rename(__state.FileName, __result.FileName);
        }
    }
}
