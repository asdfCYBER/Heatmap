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
    }
}
