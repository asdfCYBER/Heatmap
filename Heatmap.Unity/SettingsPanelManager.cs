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
        public Button Close;

        public Color ToggleOff;

        public Color ToggleOn;

        public Toggle ShowCustomColormap;

        public GradientManager ColormapManager;

        public TMP_Dropdown Colormap;

        public TMP_InputField MeasuringPeriod;

        public TMP_InputField BusynessMinimum;

        public TMP_InputField BusynessMaximum;

        public TMP_InputField DeleteAfter;

        public TMP_Dropdown Mode;

        public TMP_Text MinimumUnit; // label

        public TMP_Text MaximumUnit; // label

        public void Awake()
        {
            ShowCustomColormap.onValueChanged.AddListener(ToggleCustomColormap);
        }

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

        /// <summary>
        /// Toggle the custom color panel, update the toggle color
        /// </summary>
        public void ToggleCustomColormap(bool show)
        {
            ColormapManager.gameObject.SetActive(show);

            ColorBlock colors = ShowCustomColormap.colors;
            colors.normalColor = colors.selectedColor = show ? ToggleOn : ToggleOff;
            ShowCustomColormap.colors = colors;
        }
    }
}
