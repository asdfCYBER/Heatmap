using System.Collections.Generic;
using Game;
using Game.Context;
using Heatmap.Unity;
using TMPro;
using UnityEngine;

namespace Heatmap.UI
{
    public static class Tooltip
    {
        private static readonly GameObject _prefab = LoadPrefab();

        private static GameObject _tooltipInstance;

        private static GameObject LoadPrefab()
        {
            // Load assets from assetbundle
            if (Heatmap.HeatmapUIAssets is null)
                return null;

            // Load prefab from assets and return
            return Heatmap.HeatmapUIAssets.LoadAsset<GameObject>("Tooltip");
        }

        /// <summary>
        /// Show the tooltip, create one if there is not one yet
        /// </summary>
        public static void Show()
        {
            // Destroy it or do nothing if the tooltip is not allowed to exist
            if (!(ToolbarButton.Value && Heatmap.Instance.AllowOverlay))
            {
                Destroy();
                return;
            }

            // Create a tooltip if it does not exist
            if (_tooltipInstance == null)
            {
                Canvas uiCanvas = Ctx.Deps.GameButtons.GetComponentInParent<Canvas>();
                _tooltipInstance = UnityEngine.Object.Instantiate(_prefab,
                    uiCanvas.transform, worldPositionStays: false);
                _tooltipInstance.GetComponent<TooltipManager>().CanvasRect = 
                    uiCanvas.gameObject.GetComponent<RectTransform>();
            }

            _tooltipInstance.SetActive(true);
        }

        /// <summary>
        /// Make the tooltip invisible and uninteractable
        /// </summary>
        public static void Hide()
        {
            if (_tooltipInstance != null)
                _tooltipInstance.SetActive(false);
        }

        public static void Destroy()
        {
            if (_tooltipInstance != null)
                UnityEngine.Object.Destroy(_tooltipInstance);
        }

        /// <summary>
        /// Update the text on the tooltip to <paramref name="text"/>
        /// </summary>
        public static void UpdateText(string text)
        {
            if (_tooltipInstance == null)
                return;

            _tooltipInstance.GetComponent<TooltipManager>().Text.text = text;
        }
    }
}
