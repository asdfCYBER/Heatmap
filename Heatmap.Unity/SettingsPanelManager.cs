using UnityEngine;

namespace Heatmap.Unity
{
    /// <summary>
    /// Unity component which holds references to objects
    /// the main assembly needs to do things with
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SettingsPanelManager : MonoBehaviour
    {
        [SerializeField]
        public GameObject CloseButton;

        [SerializeField]
        public GameObject ColormapDropdown;

        [SerializeField]
        public GameObject MeasuringPeriodInputField;

        [SerializeField]
        public GameObject BusynessMultiplierInputField;
    }
}
