using UnityEngine;
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
        public GameObject Close; // button

        [SerializeField]
        public GameObject Colormap; // dropdown

        [SerializeField]
        public GameObject MeasuringPeriod; // inputfield

        [SerializeField]
        public GameObject BusynessMinimum; // inputfield

        [SerializeField]
        public GameObject BusynessMaximum; // inputfield

        [SerializeField]
        public GameObject DeleteAfter; // inputfield

        [SerializeField]
        public GameObject Mode; // dropdown

        [SerializeField]
        public GameObject MinimumUnit; // label

        [SerializeField]
        public GameObject MaximumUnit; // label

        /// <summary>
        /// Set the correct units (minutes, km/h, etc) for the current mode
        /// </summary>
        public void ModeChanged(int value)
        {
            string mode = Mode.GetComponent<TMP_Dropdown>().options[value].text.ToLower();
            string unit = "<color=\"red\">error</color>";

            if (mode.Contains("time"))
                unit = "minutes";
            else if (mode.Contains("visits"))
                unit = "times"; // technically not a unit
            else if (mode.Contains("speed"))
                unit = "km/h";
            else if (mode.Contains("length"))
                unit = "km";

            MinimumUnit.GetComponent<TMP_Text>().text = unit;
            MaximumUnit.GetComponent<TMP_Text>().text = unit;
        }

        public void UpdateUnits() =>
            ModeChanged(Mode.GetComponent<TMP_Dropdown>().value);
    }
}
