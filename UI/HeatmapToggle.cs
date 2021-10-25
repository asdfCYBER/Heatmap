using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap.UI
{
    /// <summary>
    /// Placeholder toggle using IMGUI
    /// </summary>
    public class HeatmapToggle : MonoBehaviour
    {
        public static bool HeatmapEnabled = false;

        public void OnGUI()
        {
            bool newValue = GUI.Toggle(new Rect(100, Screen.height - 300, 100, 100), HeatmapEnabled, "Heatmap");
            if (newValue != HeatmapEnabled)
            {
                HeatmapEnabled = newValue;
                Log($"Heatmap has been {(newValue ? "enabled" : "disabled")}", LogLevel.Debug);
                Heatmap.Instance.RefreshAllNodes();
            }
        }
    }
}
