using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        public Button Close;

        [SerializeField]
        public TMP_Dropdown Colormap;

        [SerializeField]
        public TMP_InputField MeasuringPeriod;

        [SerializeField]
        public TMP_InputField BusynessMinimum;

        [SerializeField]
        public TMP_InputField BusynessMaximum;

        [SerializeField]
        public TMP_InputField DeleteAfter;

        [SerializeField]
        public TMP_Dropdown Mode;

        [SerializeField]
        public TMP_Text MinimumUnit; // label

        [SerializeField]
        public TMP_Text MaximumUnit; // label

        /// <summary>
        /// Set the correct units (minutes, km/h, etc) for the current mode
        /// </summary>
        public void ModeChanged(int value)
        {
            string mode = Mode.options[value].text.ToLower();
            string unit = "<color=\"red\">error</color>";

            if (mode.Contains("time"))
                unit = "minutes";
            else if (mode.Contains("visits"))
                unit = "times"; // technically not a unit
            else if (mode.Contains("speed"))
                unit = "km/h";
            else if (mode.Contains("length"))
                unit = "meters";

            MinimumUnit.text = unit;
            MaximumUnit.text = unit;
        }

        public void UpdateUnits() =>
            ModeChanged(Mode.value);
    }
}
