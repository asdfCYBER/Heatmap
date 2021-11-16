using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    /// <summary>
    /// Calls <see cref="Heatmap.AutoRefreshAllNodes"/>
    /// every Interval amount of seconds
    /// </summary>
    public class AutoRefreshTimer : MonoBehaviour
    {
        public float TimeNext { get; private set; } = Time.time;

        public float Interval { get; set; } = 1;

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Used by Unity")]
        private void Update()
        {
            if (Time.time > TimeNext)
            {
                TimeNext = Time.time + Interval;
                Heatmap.Instance.AutoRefreshAllNodes();
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Used by Unity")]
        private void OnDisable() =>
            Log("An AutoRefreshTimer script was disabled", LogLevel.Info);
    }
}
