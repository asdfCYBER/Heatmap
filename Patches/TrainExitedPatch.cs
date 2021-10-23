using System.Diagnostics.CodeAnalysis;
using Game.Railroad;
using Game.Train;
using HarmonyLib;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch Game.Railroad.Node.AfterTrainExited to detect trains exiting nodes
    /// </summary>
    [HarmonyPatch(typeof(Node), "AfterTrainExited")]
    internal class TrainExitedPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(Train train, Node __instance)
        {
            Log($"Train with uuid {train.Uuid} has exited node {__instance.FriendlyName}", LogLevel.Debug);
            NodeTimerTracker.Instance.RegisterNodeExited(__instance);
        }
    }
}
