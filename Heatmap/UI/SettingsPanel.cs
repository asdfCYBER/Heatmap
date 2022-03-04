using System.Linq;
using Game;
using Heatmap.IO;
using Heatmap.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Heatmap.Logging.Logging;

namespace Heatmap.UI
{
    public class SettingsPanel
    {
        public static SettingsPanel Instance { get; private set; }

        private static readonly GameObject _prefab = LoadPrefab();

        private readonly GameObject _panel;

        private readonly Button _closeButton;

        private readonly TMP_Dropdown _colormap;

        private readonly TMP_InputField _measuringPeriod;

        private readonly TMP_InputField _busynessMinimum;

        private readonly TMP_InputField _busynessMaximum;

        private readonly TMP_InputField _deleteAfter;

        private readonly TMP_Dropdown _mode;

        private string _prevDeleteAfterValue;

        private int? _nextDeleteAfterValue;

        public SettingsPanel(GameObject parent)
        {
            // Instantiate prefab and find UI elements
            _panel = UnityEngine.Object.Instantiate(_prefab, parent.transform, worldPositionStays: false);
            SettingsPanelManager panelManager = _panel.GetComponent<SettingsPanelManager>();
            
            _closeButton = panelManager.Close.GetComponent<Button>();
            _colormap = panelManager.Colormap.GetComponent<TMP_Dropdown>();
            _measuringPeriod = panelManager.MeasuringPeriod.GetComponent<TMP_InputField>();
            _busynessMinimum = panelManager.BusynessMinimum.GetComponent<TMP_InputField>();
            _busynessMaximum = panelManager.BusynessMaximum.GetComponent<TMP_InputField>();
            _deleteAfter = panelManager.DeleteAfter.GetComponent<TMP_InputField>();
            _mode = panelManager.Mode.GetComponent<TMP_Dropdown>();

            // Move the panel so it is next to the toolbar
            _panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -180);

            // Hook up events
            _closeButton.onClick.AddListener(Destroy);
            _colormap.onValueChanged.AddListener(ColormapSelected);
            _measuringPeriod.onEndEdit.AddListener(MeasuringPeriodChanged);
            _busynessMinimum.onEndEdit.AddListener(BusynessMinimumChanged);
            _busynessMaximum.onEndEdit.AddListener(BusynessMaximumChanged);
            _deleteAfter.onSelect.AddListener(delegate (string value) { _prevDeleteAfterValue = value; });
            _deleteAfter.onEndEdit.AddListener(DeleteAfterChanged);
            _mode.onValueChanged.AddListener(ModeSelected);

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
            _colormap.ClearOptions();
            _colormap.AddOptions(ColorGradient.Gradients.Keys.ToList());
            _colormap.value = _colormap.options.FindIndex(
                option => option.text == Settings.Instance.GradientName);

            // add all modes as options to the dropdown
            _mode.ClearOptions();
            _mode.AddOptions(Overlay.Modes.Keys.ToList());
            _mode.value = _mode.options.FindIndex(
                option => option.text == Settings.Instance.Mode);

            // set measuring period and busyness multiplier values
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            _measuringPeriod.text = Settings.Instance.MeasuringPeriod.ToString();
            _busynessMinimum.text = boundaries.Minimum.ToString();
            _busynessMaximum.text = boundaries.Maximum.ToString();
            _deleteAfter.text = Settings.Instance.DeleteAfter.ToString();
        }

        public static void Show()
        {
            Log("Opening settings panel", LogLevel.Info);
            Instance = new SettingsPanel(GameController.Current.UiCanvas.gameObject);
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
            Log($"Colormap changed to {_colormap.options[index].text}", LogLevel.Info);
            Settings.Instance.GradientName = _colormap.options[index].text;
            Heatmap.Instance.RefreshAllNodes();
        }

        private void MeasuringPeriodChanged(string value)
        {
            Log($"Measuring period changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(_measuringPeriod.text, out int measureperiod)
                && int.TryParse(_deleteAfter.text, out int deleteperiod))
            {
                // ensure that measureperiod <- deleteperiod
                if (measureperiod > deleteperiod)
                {
                    measureperiod = deleteperiod;
                    _measuringPeriod.text = measureperiod.ToString();
                }
                
                Settings.Instance.MeasuringPeriod = measureperiod;
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void BusynessMinimumChanged(string value)
        {
            Log($"Busyness minimum changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(_busynessMinimum.text, out int min) 
                && int.TryParse(_busynessMaximum.text, out int max))
            {
                // ensure that min < max
                if (min >= max)
                {
                    min = max - 1;
                    _busynessMinimum.text = min.ToString();
                }

                Settings.Instance.BoundaryValues.SetCurrentMinimum(min);
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void BusynessMaximumChanged(string value)
        {
            Log($"Busyness maximum changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(_busynessMinimum.text, out int min)
                && int.TryParse(_busynessMaximum.text, out int max))
            {
                // ensure that min < max
                if (max <= min)
                {
                    max = min + 1;
                    _busynessMaximum.text = max.ToString();
                }

                Settings.Instance.BoundaryValues.SetCurrentMaximum(max);
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void DeleteAfterChanged(string value)
        {
            Log($"Delete after changed to {value} minutes", LogLevel.Info);

            if (int.TryParse(_deleteAfter.text, out int result))
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
        /// <param name="result"></param>
        private void DeleteAfterChangedDialogResult(DialogResult result)
        {
            Log($"Delete after changes confirmed: {result}", LogLevel.Info);
            EnableControls();

            if (result == DialogResult.Yes && _nextDeleteAfterValue.HasValue)
            {
                // ensure that measureperiod <= _nextDataAfterValue.Value
                if (Settings.Instance.MeasuringPeriod > _nextDeleteAfterValue.Value)
                {
                    _measuringPeriod.text = _nextDeleteAfterValue.Value.ToString();
                    Settings.Instance.MeasuringPeriod = _nextDeleteAfterValue.Value;
                }

                Settings.Instance.DeleteAfter = _nextDeleteAfterValue.Value;
            }
            else
                _deleteAfter.text = _prevDeleteAfterValue;
        }

        /// <summary>
        /// Disable all inputs on the settingspanel so that only the confirm dialog can be used
        /// </summary>
        public void DisableControls()
        {
            _colormap.enabled = false;
            _measuringPeriod.enabled = false;
            _busynessMinimum.enabled = false;
            _busynessMaximum.enabled = false;
            _deleteAfter.enabled = false;
        }

        /// <summary>
        /// (Re-)enable all inputs on the settingspanel
        /// </summary>
        public void EnableControls()
        {
            _colormap.enabled = true;
            _measuringPeriod.enabled = true;
            _busynessMinimum.enabled = true;
            _busynessMaximum.enabled = true;
            _deleteAfter.enabled = true;
        }

        private void ModeSelected(int index)
        {
            Log($"Colormap changed to {_mode.options[index].text}", LogLevel.Info);
            Settings.Instance.Mode = _mode.options[index].text;
            Boundaries boundaries = Settings.Instance.BoundaryValues.GetCurrentBoundaries();
            _busynessMinimum.text = boundaries.Minimum.ToString();
            _busynessMaximum.text = boundaries.Maximum.ToString();
            Heatmap.Instance.RefreshAllNodes();
        }
    }
}
