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

        private readonly TMP_InputField _busynessMultiplier;

        public SettingsPanel(GameObject parent)
        {
            // Instantiate prefab and find UI elements
            _panel = UnityEngine.Object.Instantiate(_prefab, parent.transform, worldPositionStays: false);
            SettingsPanelManager panelManager = _panel.GetComponent<SettingsPanelManager>();
            
            _closeButton = panelManager.CloseButton.GetComponent<Button>();
            _colormap = panelManager.ColormapDropdown.GetComponent<TMP_Dropdown>();
            _measuringPeriod = panelManager.MeasuringPeriodInputField.GetComponent<TMP_InputField>();
            _busynessMultiplier = panelManager.BusynessMultiplierInputField.GetComponent<TMP_InputField>();

            // Move the panel so it is next to the toolbar
            _panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -180);

            // Hook up events
            _closeButton.onClick.AddListener(Destroy);
            _colormap.onValueChanged.AddListener(ColormapSelected);
            _measuringPeriod.onEndEdit.AddListener(MeasuringPeriodChanged);
            _busynessMultiplier.onEndEdit.AddListener(BusynessMultiplierChanged);

            InitializeValues();
        }

        private static GameObject LoadPrefab()
        {
            // Load assets from file
            AssetBundle assetBundle = IO.Utils.LoadAssetBundle("HeatmapUIassets");
            if (assetBundle == null)
            {
                Log("The HeatmapUIassets file could not be loaded! " +
                    "Settings will not be visible.", LogLevel.Exception);
                return null;
            }

            // Load prefab from assets and return
            return assetBundle.LoadAsset<GameObject>("Settings panel");
        }

        private void InitializeValues()
        {
            // add all registered gradients as options to the dropdown, and set cividis as default
            _colormap.ClearOptions();
            _colormap.AddOptions(ColorGradient.Gradients.Keys.ToList());
            _colormap.value = _colormap.options.FindIndex(
                option => option.text == Settings.Instance.GradientName);

            // set measuring period and busyness multiplier values
            _measuringPeriod.text = Settings.Instance.MeasuringPeriod.ToString();
            _busynessMultiplier.text = Settings.Instance.BusynessMultiplier.ToString();
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
            Log($"Colormap {_colormap.options[index].text} selected", LogLevel.Info);
            Settings.Instance.GradientName = _colormap.options[index].text;
            Heatmap.Instance.RefreshAllNodes();
        }

        private void MeasuringPeriodChanged(string value)
        {
            Log($"Measuring period changed to {value}", LogLevel.Info);

            if (int.TryParse(Instance?._measuringPeriod.text, out int result))
            {
                Settings.Instance.MeasuringPeriod = result;
                Heatmap.Instance.RefreshAllNodes();
            }
        }

        private void BusynessMultiplierChanged(string value)
        {
            Log($"Busyness multiplier changed to {value}", LogLevel.Info);

            if (int.TryParse(Instance?._busynessMultiplier.text, out int result))
            {
                Settings.Instance.BusynessMultiplier = result;
                Heatmap.Instance.RefreshAllNodes();
            }
        }
    }
}
