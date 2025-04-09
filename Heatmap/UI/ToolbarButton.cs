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

        private static int _pressedTimes = 0;

        /// <summary>
        /// Add a heatmap toggle button to the interface
        /// </summary>
        internal static void Create(InterfaceConfigurationPanelView panel)
        {
            if (_button != null) return;

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
                _button.IconCode = "\uF06D";

                // Make the button do button things
                _button.OnClick.RemoveAllListeners();
                _button.OnClick.AddListener(ButtonPressed);
                _button.ClearHighlight();
                _button.SetIsOnWithoutNotify(false);
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

            try
            {
                _button.SetIsOnWithoutNotify(Value);
            }
            catch (NullReferenceException e)
            {
                Log("The button was null", LogLevel.Warning);
            }
        }
    }
}
