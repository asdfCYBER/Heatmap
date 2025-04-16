using UnityEngine;
using UnityEngine.EventSystems;

namespace Logviewer.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class Draggable : MonoBehaviour, IDragHandler
    {
        private RectTransform _panelTransform;

        public void Start()
        {
            _panelTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData data)
        {
            // Move gameobject the same amount as the mouse moved
            _panelTransform.position += new Vector3(data.delta.x, data.delta.y, 0);
        }
    }
}
