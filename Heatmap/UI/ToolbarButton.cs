using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Game.Hud.InGame.ConfigurationPanel.Variants;
using TMPro;
using UIElements;
using static Heatmap.Logging.Logging;

namespace Heatmap.UI
{
    public class ToolbarButton
    {
        public static ToolbarButton Instance { get; private set; }

        /// <summary>
        /// True if the heatmap button is toggled, otherwise false
        /// </summary>
        public static bool Value { get; private set; } = false;

        private static ToggleButton _button;

        internal static int _pressedTimes = 0;

        /// <summary>
        /// Add a heatmap toggle button to the interface
        /// </summary>
        internal static void Create(InterfaceConfigurationPanelView panel)
        {
            Transform content = panel.GetComponentInChildren<ScrollRect>().content.transform;
            try
            {
                // Clone one of the panel items
                Transform section = content.Find("Global Settings/UI Section");
                GameObject prefab = section.Find("Toggle Station Colors").gameObject;
                GameObject clone = GameObject.Instantiate(prefab, section);
                _button = clone.GetComponent<ToggleButton>();
                clone.SetActive(true);

                // Remove existing LocalizeStringEvents
                foreach (LocalizeStringEvent localize in clone.GetComponentsInChildren<LocalizeStringEvent>())
                    GameObject.Destroy(localize);

                // Set the label and icon, remove localization I think
                Transform label = clone.transform.Find("Label holder/Text label");
                label.GetComponent<TextMeshProUGUI>().text = "Toggle heatmap (hotkey: <b>h</b>)\nDouble click to show settings";
                Game.Hud.Tooltip tooltip = label.GetComponent<Game.Hud.Tooltip>();
                tooltip.TooltipText = "Toggle heatmap (hotkey: <b>h</b>)\nDouble click to show settings";
                tooltip.LocalizedText = new LocalizedString();
                tooltip.DynamicLocalizedText = new Utils.CachedLocalizedString();
                _button.IconCode = "\uF06D";

                // Make the button do button things
                _button.OnClick.RemoveAllListeners();
                _button.OnClick.AddListener(ButtonPressed);
            }
            catch (NullReferenceException e)
            {
                Log($"Button could not be added: {e.Message}", LogLevel.Exception);
            }
        }

        internal static async void ButtonPressed()
        {
            _pressedTimes += 1;
            
            await Task.Delay(200);

            if (_pressedTimes == 1)
                ToggleValue(); 
            else if (_pressedTimes == 2)
                SettingsPanel.ToggleShow();
            
            _pressedTimes = 0;
        }

        public static void ToggleValue()
        {
            Value = !Value;
            Log($"Heatmap has been {(Value ? "enabled" : "disabled")}", LogLevel.Info);
            Heatmap.Instance.RefreshAllNodes();

            if (Value)
                _button.Highlight();
            else
                _button.ClearHighlight();
        }

        /// <summary>
        /// Remove localization events, change the tooltip and icon
        /// </summary>
        private void CustomiseButton()
        {
            // Remove existing LocalizeStringEvents
            foreach (LocalizeStringEvent localize in _button.GetComponentsInChildren<LocalizeStringEvent>())
                GameObject.Destroy(localize);
            
            // Change the tooltip
            Game.Hud.Tooltip tooltip = _button.GetComponentInParent<Game.Hud.Tooltip>();
            tooltip.TooltipText = "Toggle heatmap (hotkey: <b>h</b>)\nDouble click to show settings";
            tooltip.LocalizedText = new LocalizedString();

            // Change the icon
            TextMeshProUGUI tmpComponent = _button.GetComponentInChildren<TextMeshProUGUI>();
            tmpComponent.text = "\uF06D"; // fa-regular fa-fire (font awesome 6 icon)
        }
    }
}
