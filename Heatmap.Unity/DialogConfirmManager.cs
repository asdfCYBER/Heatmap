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
    public class DialogConfirmManager : MonoBehaviour
    {
        [SerializeField]
        public TMP_Text Title;

        [SerializeField]
        public TMP_Text Text;

        [SerializeField]
        public Button Cancel;

        [SerializeField]
        public Button Yes;

        [SerializeField]
        public Button No;
    }
}
