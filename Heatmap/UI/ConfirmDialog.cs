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
        public DialogConfirmManager DialogManager { get; }

        private static readonly GameObject _prefab = LoadPrefab();

        private readonly GameObject _dialog;

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
            DialogManager = _dialog.GetComponent<DialogConfirmManager>();

            // Set position
            _dialog.GetComponent<RectTransform>().anchoredPosition = position;

            // Hook up events
            DialogManager.Cancel.onClick.AddListener(OnClickCancel);
            DialogManager.Yes.onClick.AddListener(OnClickYes);
            DialogManager.No.onClick.AddListener(OnClickNo);

            // Initialize fields
            DialogManager.Title.text = title;
            DialogManager.Text.text = text;
        }

        private static GameObject LoadPrefab()
        {
            // Load assets from assetbundle
            if (Heatmap.HeatmapUIAssets == null)
                return null;

            // Load prefab from assets and return
            return Heatmap.HeatmapUIAssets.LoadAsset<GameObject>("Confirm dialog");
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
