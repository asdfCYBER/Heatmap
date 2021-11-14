using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using Game.Hud;
using Game.Hud.Menu;
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

        private readonly MultiTargetButton _button;

        private readonly GameButton _buttonHolder;

        /// <summary>
        /// Add a heatmap toggle button to bottom of the left toolbar.
        /// Credit to Yarian for figuring out what to copy and how to edit it!
        /// </summary>
        public ToolbarButton()
        {
            // Find the button that will be copied, and the layoutgroup it is in
            ModalDialogUiController modalDialog = Heatmap._controller.GameController.GetModalDialog();
            GameButton sourceButton = modalDialog.GameButtons.ContractsButtonHolder;
            VerticalLayoutGroup buttonGroup = sourceButton.GetComponentInParent<VerticalLayoutGroup>();

            // Copy the button and add it to the layoutgroup
            _buttonHolder = Object.Instantiate(sourceButton);
            Object.DontDestroyOnLoad(_buttonHolder);
            _buttonHolder.transform.SetParent(buttonGroup.transform, worldPositionStays: false);
            _buttonHolder.gameObject.SetActive(value: true);
            _button = _buttonHolder.GetComponentInChildren<MultiTargetButton>();

            CustomiseButton();

            // Make the button a toggle
            _button.onClick = new Button.ButtonClickedEvent();
            _button.onClick.AddListener(ToggleValue);
            _button.onRightClick.AddListener(SettingsPanel.ToggleShow);

            // Not entirely sure what this exactly does, but it increases the
            // layoutgroup size so that buttons are no longer squished
            RectTransform rect = buttonGroup.gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y * 1.5f);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y * 1.5f);
        }

        public static void Show()
        {
            Destroy(); // destroy existing button if there is one
            Instance = new ToolbarButton();
        }

        public static void Destroy()
        {
            if (Instance != null)
                Object.Destroy(Instance._buttonHolder);
        }

        private void ToggleValue()
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
                Object.Destroy(localize);
            
            // Change the tooltip
            Tooltip tooltip = _button.GetComponentInParent<Tooltip>();
            tooltip.TooltipText = "Toggle heatmap";
            tooltip.Text = "Toggle heatmap";
            tooltip.LocalizedText = new LocalizedString();

            // Change the icon
            TextMeshProUGUI tmpComponent = _button.GetComponentInChildren<TextMeshProUGUI>();
            tmpComponent.text = "\uF7E4"; // fire-alt font awesome 5 icon
        }
    }
}
