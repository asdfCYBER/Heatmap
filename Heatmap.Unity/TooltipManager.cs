using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Heatmap.Unity
{
    public class TooltipManager : MonoBehaviour
    {
        public RectTransform CanvasRect;

        public RectTransform TooltipRect;

        public TMP_Text Text;

        public Vector2 Offset;

        public void Update()
        {
            if (CanvasRect == null)
               return;

            // Get mouse position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                CanvasRect, Mouse.current.position.ReadValue(), null, out Vector2 localPosition);

            // For some reason this is necessary
            Vector2 canvasOffset = new Vector2(CanvasRect.rect.width / 2, -CanvasRect.rect.height / 2);

            // Set tooltip position
            TooltipRect.anchoredPosition = localPosition + Offset + canvasOffset;
        }
    }
}
