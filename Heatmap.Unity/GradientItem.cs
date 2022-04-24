using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Heatmap.Unity
{
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class GradientItem : MonoBehaviour
    {
        public GradientManager Gradient;

        public Toggle Background;

        public Color BackgroundOff;

        public Color BackgroundOn;

        public TMP_InputField KeyInput;

        [NonSerialized]
        public float Key = 0;

        public TMP_InputField HexInput;

        [NonSerialized]
        public string Hex = "#FFFFFF";

        public Image ColorPreview;

        public void Awake()
        {
            // Hook up events
            GetComponent<Image>().color = BackgroundOff;
            Background.onValueChanged.AddListener(OnValueChanged);
            KeyInput.onSelect.AddListener(delegate (string s) { Select(); });
            HexInput.onSelect.AddListener(delegate (string s) { Select(); });

            // Register current values
            OnKeyChange(KeyInput.text);
            OnHexChange(HexInput.text);
        }

        /// <summary>
        /// Validate <see cref="HexInput"/> input and update <see cref="ColorPreview"/> and <see cref="Hex"/>
        /// </summary>
        /// <param name="value"></param>
        public void OnHexChange(string value)
        {
            Match match = Regex.Match(value.Trim(), @"^#?(?:[0-9A-F]{6}|[0-9A-F]{3})$", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string matched = match.Value;

                if (!matched.StartsWith("#"))
                    matched = "#" + matched;
                
                Hex = HexInput.text = matched.ToUpper();

                if (ColorUtility.TryParseHtmlString(matched, out Color color))
                {
                    ColorPreview.color = color;
                    Gradient.UpdatePreview();
                }
            }
            else
            {
                HexInput.text = Hex;
            }    
        }

        /// <summary>
        /// Validate <see cref="KeyInput"/> input and update <see cref="Key"/>
        /// </summary>
        /// <param name="value"></param>
        public void OnKeyChange(string value)
        {
            Match match = Regex.Match(value.Trim(), @"^\d+(?:\.\d+)?%?$");
            string matched = match.Value;

            if (match.Success && float.TryParse(matched, out float result) && result <= 100)
            {
                Key = result;

                if (!matched.EndsWith("%"))
                    matched += "%";

                KeyInput.text = matched;
                Gradient.SortItems();
                Gradient.UpdatePreview();
            }
            else
            {
                KeyInput.text = Key.ToString() + "%";
            }
        }

        /// <summary>
        /// Update the background color when the item is (de)selected
        /// </summary>
        /// <param name="value"></param>
        public void OnValueChanged(bool value)
        {
            GetComponent<Image>().color = value ? BackgroundOn : BackgroundOff;
        }

        public void Select()
        {
            Background.isOn = true;
            OnValueChanged(true); // Doesn't always seem to happen so do it manually
        }

        public void Deselect()
        {
            Background.isOn = false;
            OnValueChanged(false); // Doesn't always seem to happen so do it manually
        }
    }
}
