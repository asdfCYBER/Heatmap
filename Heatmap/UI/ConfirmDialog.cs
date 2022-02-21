using System;
using System.Linq;
using Heatmap.IO;
using Heatmap.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Heatmap.Logging.Logging;

namespace Heatmap.UI
{
    public enum DialogResult
    {
        None,
        Yes,
        No,
        Cancel
    }

    public class ConfirmDialog
    {
        private static readonly GameObject _prefab = LoadPrefab();

        private readonly GameObject _dialog;

        private readonly TMP_Text _title;

        private readonly TMP_Text _text;

        private readonly Button _buttonCancel;

        private readonly Button _buttonYes;

        private readonly Button _buttonNo;

        private readonly Action<DialogResult> _result;

        /// <summary>
        /// Create a new yes/no dialog
        /// </summary>
        /// <param name="parent">The GameObject the dialog will be a child of</param>
        /// <param name="position">The position relative to the <paramref name="parent"/></param>
        /// <param name="result">The method to send this dialog's result to</param>
        /// <param name="title">Title of the dialog</param>
        /// <param name="text">Text (body) of the dialog</param>
        public ConfirmDialog(GameObject parent, Vector2 position,
            Action<DialogResult> result, string title, string text)
        {
            _dialog = UnityEngine.Object.Instantiate(_prefab, parent.transform, worldPositionStays: false);
            _result = result;

            // Set fields
            DialogConfirmManager panelManager = _dialog.GetComponent<DialogConfirmManager>();
            _title = panelManager.Title.GetComponent<TMP_Text>();
            _text = panelManager.Text.GetComponent<TMP_Text>();
            _buttonCancel = panelManager.Cancel.GetComponent<Button>();
            _buttonNo = panelManager.No.GetComponent<Button>();
            _buttonYes = panelManager.Yes.GetComponent<Button>();

            // Set position
            _dialog.GetComponent<RectTransform>().anchoredPosition = position;

            // Hook up events
            _buttonCancel.onClick.AddListener(OnClickCancel);
            _buttonYes.onClick.AddListener(OnClickYes);
            _buttonNo.onClick.AddListener(OnClickNo);

            // Initialize fields
            _title.text = title;
            _text.text = text;
        }

        private static GameObject LoadPrefab()
        {
            // Load assets from assetbundle
            if (Heatmap.HeatmapUIAssets == null)
                return null;

            // Load prefab from assets and return
            return Heatmap.HeatmapUIAssets.LoadAsset<GameObject>("Confirm prompt");
        }

        private void OnClickCancel()
        {
            Log("Confirm dialog cancel", LogLevel.Info);
            UnityEngine.Object.Destroy(_dialog);
            _result(DialogResult.Cancel);
        }

        private void OnClickYes()
        {
            Log("Confirm dialog yes", LogLevel.Info);
            UnityEngine.Object.Destroy(_dialog);
            _result(DialogResult.Yes);
        }

        private void OnClickNo()
        {
            Log("Confirm dialog no", LogLevel.Info);
            UnityEngine.Object.Destroy(_dialog);
            _result(DialogResult.No);
        }
    }
}
