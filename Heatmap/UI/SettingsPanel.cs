using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Context;
using Heatmap.IO;
using Heatmap.Unity;
using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap.UI
{
    public class SettingsPanel
    {
        public static SettingsPanel Instance { get; private set; }

        public SettingsPanelManager PanelManager { get; }

        private static readonly GameObject _prefab = LoadPrefab();

        private readonly GameObject _panel;

        private string _prevDeleteAfterValue;

        private int? _nextDeleteAfterValue;

        public SettingsPanel(GameObject parent)
        {
            // Instantiate prefab and find UI elements
            _panel = UnityEngine.Object.Instantiate(_prefab, parent.transform, worldPositionStays: false);
            PanelManager = _panel.GetComponent<SettingsPanelManager>();

            // Move the panel so it is next to the toolbar
            _panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -180);

            // Hook up events
            PanelManager.Close.onClick.AddListener(Destroy);
            PanelManager.Colormap.onValueChanged.AddListener(ColormapSelected);
            PanelManager.MeasuringPeriod.onEndEdit.AddListener(MeasuringPeriodChanged);
            PanelManager.BusynessMinimum.onEndEdit.AddListener(BusynessMinimumChanged);
            PanelManager.BusynessMaximum.onEndEdit.AddListener(BusynessMaximumChanged);
            PanelManager.DeleteAfter.onSelect.AddListener(delegate (string value) { _prevDeleteAfterValue = value; });
            PanelManager.DeleteAfter.onEndEdit.AddListener(DeleteAfterChanged);
            PanelManager.Mode.onValueChanged.AddListener(ModeSelected);

            PanelManager.ColormapManager.Name.onEndEdit.AddListener(VerifyColormapName);
            PanelManager.ColormapManager.Delete.onClick.AddListener(DeleteColormapOption);
            PanelManager.ColormapManager.Save.onClick.AddListener(AddColormapOption);

            InitializeValues();
        }

        private static GameObject LoadPrefab()
        {
            // Load assets from assetbundle
            if (Heatmap.HeatmapUIAssets == null)
                return null;

            // Load prefab from assets and return
            return Heatmap.HeatmapUIAssets.LoadAsset<GameObject>("Settings panel");
        }

        private void InitializeValues()
        {
            // add all registered gradients as options to the dropdown
            PanelManager.Colormap.ClearOptions();
            PanelManager.Colormap.AddOptions(ColorGradient.Gradients.Keys.ToList());
            PanelManager.Colormap.value = PanelManager.Colormap.options.FindIndex(
                option => option.text == Settings.Instance.GradientName);

            // add all modes as options to the dropdown
            PanelManager.Mode.ClearOptions();
            PanelManager.Mode.AddOptions(Overlay.Modes.Keys.ToList());
            PanelManager.Mode.value = PanelManager.Mode.options.FindIndex(
                option => option.text == Settings.Instance.Mode);

            // set measuring period and busyness multiplier values
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            PanelManager.MeasuringPeriod.text = Settings.Instance.MeasuringPeriod.ToString();
            PanelManager.BusynessMinimum.text = boundaries.Minimum.ToString();
            PanelManager.BusynessMaximum.text = boundaries.Maximum.ToString();
            PanelManager.DeleteAfter.text = Settings.Instance.DeleteAfter.ToString();
        }

        public static void Show()
        {
            Log("Opening settings panel", LogLevel.Info);
            Instance = new SettingsPanel(Ctx.Deps.GameButtons.GetComponentInParent<Canvas>().gameObject);
        }

        public static void Destroy()
        {
            Log("Closing settings panel", LogLevel.Info);
            SettingsIO.Save(Settings.Instance);
            Object.Destroy(Instance._panel);
        }

        public static void ToggleShow()
        {
            if (Instance?._panel == null)
                Show();
            else
                Destroy();
        }

        private void ColormapSelected(int index)
        {
            Log($"Colormap changed to {PanelManager.Colormap.options[index].text}", LogLevel.Info);
            Settings.Instance.GradientName = PanelManager.Colormap.options[index].text;
            PanelManager.ColormapManager.Load(Settings.Instance.Gradient.Gradient, Settings.Instance.GradientName);
            Heatmap.Instance.RefreshAllNodes();
        }

        private void MeasuringPeriodChanged(string value)
        {
            Log($"Measuring period changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(PanelManager.MeasuringPeriod.text, out int measureperiod)
                && int.TryParse(PanelManager.DeleteAfter.text, out int deleteperiod))
            {
                // ensure that measureperiod <- deleteperiod
                if (measureperiod > deleteperiod)
                {
                    measureperiod = deleteperiod;
                    PanelManager.MeasuringPeriod.text = measureperiod.ToString();
                }
                
                Settings.Instance.MeasuringPeriod = measureperiod;
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void BusynessMinimumChanged(string value)
        {
            Log($"Busyness minimum changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(PanelManager.BusynessMinimum.text, out int min) 
                && int.TryParse(PanelManager.BusynessMaximum.text, out int max))
            {
                // ensure that min < max
                if (min >= max)
                {
                    min = max - 1;
                    PanelManager.BusynessMinimum.text = min.ToString();
                }

                Settings.Instance.BoundaryValues.SetCurrentMinimum(min);
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void BusynessMaximumChanged(string value)
        {
            Log($"Busyness maximum changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(PanelManager.BusynessMinimum.text, out int min)
                && int.TryParse(PanelManager.BusynessMaximum.text, out int max))
            {
                // ensure that min < max
                if (max <= min)
                {
                    max = min + 1;
                    PanelManager.BusynessMaximum.text = max.ToString();
                }

                Settings.Instance.BoundaryValues.SetCurrentMaximum(max);
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void DeleteAfterChanged(string value)
        {
            Log($"Delete after changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(PanelManager.DeleteAfter.text, out int result))
            {
                // Ask for confirmation if the deletion period should be sthorter
                if (result < Settings.Instance.DeleteAfter)
                {
                    _nextDeleteAfterValue = result;

                    new ConfirmDialog(_panel, new Vector2(610, 0), DeleteAfterChangedDialogResult, 
                        "Confirm changes", "Are you sure you want to delete node data sooner?" +
                        $" Any existing data older than {result} minutes will also be deleted.");

                    DisableControls();
                }
                else
                {
                    Settings.Instance.DeleteAfter = result;
                }
                
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        /// <summary>
        /// Reset the DeleteAfter inputfield if changes were cancelled,
        /// save the settings if it is confirmed
        /// </summary>
        /// <param name="result">The dialog return value</param>
        private void DeleteAfterChangedDialogResult(DialogResult result)
        {
            Log($"Delete after changes confirmed: {result}", LogLevel.Info);
            EnableControls();

            if (result == DialogResult.Yes && _nextDeleteAfterValue.HasValue)
            {
                // ensure that measureperiod <= _nextDataAfterValue.Value
                if (Settings.Instance.MeasuringPeriod > _nextDeleteAfterValue.Value)
                {
                    PanelManager.MeasuringPeriod.text = _nextDeleteAfterValue.Value.ToString();
                    Settings.Instance.MeasuringPeriod = _nextDeleteAfterValue.Value;
                }

                Settings.Instance.DeleteAfter = _nextDeleteAfterValue.Value;
            }
            else
                PanelManager.DeleteAfter.text = _prevDeleteAfterValue;
        }

        /// <summary>
        /// Disable all inputs on the settingspanel so that only the confirm dialog can be used
        /// </summary>
        private void DisableControls()
        {
            PanelManager.Colormap.enabled = false;
            PanelManager.MeasuringPeriod.enabled = false;
            PanelManager.BusynessMinimum.enabled = false;
            PanelManager.BusynessMaximum.enabled = false;
            PanelManager.DeleteAfter.enabled = false;
        }

        /// <summary>
        /// (Re-)enable all inputs on the settingspanel
        /// </summary>
        private void EnableControls()
        {
            PanelManager.Colormap.enabled = true;
            PanelManager.MeasuringPeriod.enabled = true;
            PanelManager.BusynessMinimum.enabled = true;
            PanelManager.BusynessMaximum.enabled = true;
            PanelManager.DeleteAfter.enabled = true;
        }

        private void ModeSelected(int index)
        {
            Log($"Mode changed to {PanelManager.Mode.options[index].text}", LogLevel.Info);
            Settings.Instance.Mode = PanelManager.Mode.options[index].text;
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            PanelManager.BusynessMinimum.text = boundaries.Minimum.ToString();
            PanelManager.BusynessMaximum.text = boundaries.Maximum.ToString();
            Heatmap.Instance.RefreshAllNodes();
        }

        /// <summary>
        /// Remove a colormap with the same name as in the custom colormap name inputfield from the dropdown
        /// </summary>
        public void DeleteColormapOption()
        {
            string name = PanelManager.ColormapManager.Name.text;

            // Set the selected colormap to one of the first option (a built-in colormap)
            // if the currently selected colormap is the one that has to be removed
            if (PanelManager.Colormap.captionText.text == name)
                PanelManager.Colormap.value = PanelManager.Colormap.options.FindIndex(option => option.text == "cividis");

            int amount = PanelManager.Colormap.options.RemoveAll(option => option.text == name);
            ColorGradient.Gradients.Remove(name);
            Log($"Removed {amount} colormap with the name {name}", LogLevel.Info);

            PanelManager.ColormapManager.Name.text = "";

            // Save gradients to file
            ColorGradientIO.Save();
        }

        /// <summary>
        /// Add the custom colormap as an option to the dropdown
        /// </summary>
        public void AddColormapOption()
        {
            // Create the ColorGradient (added to ColorGradient.Gradients)
            GradientManager gradient = PanelManager.ColormapManager;
            new ColorGradient(gradient.Name.text, true, gradient.GetGradient());

            PanelManager.Colormap.AddOptions(new List<string> { gradient.Name.text });
            PanelManager.Colormap.value = PanelManager.Colormap.options.Count() - 1; // Select the new option

            // Save gradients to file
            ColorGradientIO.Save();
        }

        /// <summary>
        /// Enable or disable the save and load buttons depending on the gradients <paramref name="name"/>
        /// </summary>
        private void VerifyColormapName(string name)
        {
            // Remove leading and trailing whitespace
            PanelManager.ColormapManager.Name.text = name = name.Trim();

            // Disable saving and deleting if the name is invalid
            if (string.IsNullOrEmpty(name))
                PanelManager.ColormapManager.ToggleSaveDelete(false);

            if (ColorGradient.Gradients.ContainsKey(name))
            {
                // Existing gradients can only be overwritten/deleted if editable is true
                if (!ColorGradient.Gradients[name].Editable)
                    PanelManager.ColormapManager.ToggleSaveDelete(false);
                else
                    PanelManager.ColormapManager.ToggleSaveDelete(true);
            }
            else
            {
                // Deleting is not possible but saving is if the gradient does not yet exist
                PanelManager.ColormapManager.ToggleSaveDelete(enableSave: true, enableDelete: false);
            }
        }
    }
}
