using UnityEngine;

namespace Heatmap.Unity
{
    /// <summary>
    /// Unity component which holds references to objects
    /// the main assembly needs to do things with
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class DialogConfirmManager : MonoBehaviour
    {
        [SerializeField]
        public GameObject Title; // text

        [SerializeField]
        public GameObject Text; // text

        [SerializeField]
        public GameObject Cancel; // button

        [SerializeField]
        public GameObject Yes; // button

        [SerializeField]
        public GameObject No; // button
    }
}
