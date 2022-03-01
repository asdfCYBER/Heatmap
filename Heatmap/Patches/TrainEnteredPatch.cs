using System.Diagnostics.CodeAnalysis;
using Game.Railroad;
using Game.Train;
using HarmonyLib;
using static Heatmap.Logging.Logging;

namespace Heatmap.Patches
{
    /// <summary>
    /// Patch Game.Railroad.Node.AfterTrainEntered to detect trains entering nodes
    /// </summary>
    [HarmonyPatch(typeof(Node), "AfterTrainEntered")]
    internal class TrainEnteredPatch
    {
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used by Harmony")]
        private static void Prefix(Train train, Node __instance)
        {
            Log($"Train with uuid {train.Uuid} has entered node {__instance.FriendlyName}", LogLevel.Debug);
            NodeTimerTracker.Instance.RegisterNodeEntered(__instance, trainLength: (int)train.Length);
        }
    }
}
