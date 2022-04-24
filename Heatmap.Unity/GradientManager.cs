using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Heatmap.Unity
{
    public class GradientManager : MonoBehaviour
    {
        public TMP_InputField Name;

        public RawImage GradientPreview;

        public ScrollRect Items;

        public GameObject Template;

        public Button Save;

        public Button Delete;

        public Button Add;

        public Button Remove;

        public Button MoveDown;

        public Button MoveUp;

        public void Awake()
        {
            Add.onClick.AddListener(OnClickAdd);
            Remove.onClick.AddListener(OnClickRemove);
            MoveUp.onClick.AddListener(OnClickMoveUp);
            MoveDown.onClick.AddListener(OnClickMoveDown);
            Delete.onClick.AddListener(RemoveItems);
        }

        /// <summary>
        /// Sort the gradient items on ascending keys
        /// </summary>
        public void SortItems()
        {
            List<GradientItem> items = (from item in Items.content.GetComponentsInChildren<GradientItem>()
                                        orderby item.Key
                                        select item).ToList();

            for (int i = 0; i < items.Count; i++)
                items[i].transform.SetSiblingIndex(i);
        }

        /// <summary>
        /// Create a <see cref="Gradient"/> from the <see cref="GradientItem"/>s in <see cref="Items"/>
        /// </summary>
        public Gradient GetGradient()
        {
            // For some reason creating an array and filling in the values later doesn't work
            List<GradientColorKey> colorKeys = new List<GradientColorKey>();

            for (int i = 0; i < Items.content.childCount; i++)
            {
                GradientItem item = Items.content.GetChild(i).GetComponent<GradientItem>();
                colorKeys.Add(new GradientColorKey(item.ColorPreview.color, item.Key/100));
            }

            return new Gradient { colorKeys = colorKeys.ToArray() };
        }

        /// <summary>
        /// Replace the old texture on <see cref="GradientPreview"/>
        /// with a new one created from the gradient
        /// </summary>
        public void UpdatePreview()
        {
            Gradient gradient = GetGradient();

            Rect rect = GradientPreview.GetComponent<RectTransform>().rect;
            int width = (int)rect.width;
            int height = (int)rect.height;

            // Set pixel color based on horizontal position in the texture
            Texture2D texture = new Texture2D(width, height);
            for (int i = 0; i < width; i++)
            {
                Color color = gradient.Evaluate((float)i / width);
                Color[] colorColumn = new Color[height];
                for (int j = 0; j < height; j++)
                {
                    colorColumn[j] = color;
                }
                
                texture.SetPixels(i, 0, 1, height, colorColumn);
            }

            texture.Apply();
            GradientPreview.texture = texture;
        }

        /// <summary>
        /// Duplicate the selected item or the template if nothing is selected
        /// </summary>
        private void OnClickAdd()
        {
            Toggle selectedItem = GetLastSelectedItem();

            GameObject newItem;
            if (selectedItem == null)
            {
                newItem = GameObject.Instantiate(Template, Items.content, worldPositionStays: false);
            }
            else
            {
                int index = selectedItem.transform.GetSiblingIndex();
                newItem = GameObject.Instantiate(selectedItem.gameObject, Items.content, worldPositionStays: false);
                newItem.transform.SetSiblingIndex(index + 1);
            }

            newItem.GetComponent<GradientItem>().Select();
            newItem.SetActive(true);
        }

        /// <summary>
        /// Remove the selected item
        /// </summary>
        private void OnClickRemove()
        {
            Toggle selectedItem = GetLastSelectedItem();

            if (selectedItem != null)
                Destroy(selectedItem.gameObject);
        }

        /// <summary>
        /// Swap the selected item's color value with the item above
        /// </summary>
        private void OnClickMoveUp()
        {
            GradientItem item = GetLastSelectedItem()?.GetComponent<GradientItem>();
            if (item == null)
                return;

            int index = item.transform.GetSiblingIndex();
            if (index < 1)
                return;

            GradientItem previousItem = item.transform.parent.GetChild(index - 1).GetComponent<GradientItem>();
            SwapColorAndUpdate(item, previousItem);
        }

        /// <summary>
        /// Swap the selected item's color value with the item below
        /// </summary>
        private void OnClickMoveDown()
        {
            // Swap color with item below, then select said item
            GradientItem item = GetLastSelectedItem()?.GetComponent<GradientItem>();
            if (item == null)
                return;

            int index = item.transform.GetSiblingIndex();
            if (index >= Items.content.childCount - 1)
                return;

            GradientItem nextItem = item.transform.parent.GetChild(index + 1).GetComponent<GradientItem>();
            SwapColorAndUpdate(item, nextItem);
        }

        /// <summary>
        /// Swap the color of <paramref name="itemA"/> and <paramref name="itemB"/>,
        /// then select <paramref name="itemB"/>
        /// </summary>
        private void SwapColorAndUpdate(GradientItem itemA, GradientItem itemB)
        {
            string swap = itemA.HexInput.text;
            itemA.OnHexChange(itemB.HexInput.text);
            itemB.OnHexChange(swap);
            itemB.Select();
        }

        /// <summary>
        /// Get the last/lowest active item
        /// </summary>
        private Toggle GetLastSelectedItem()
        {
            IEnumerable<Toggle> toggles = Items.GetComponent<ToggleGroup>().ActiveToggles();

            if (toggles.Count() == 0)
                return null;
            else
                return toggles.Last();
        }

        /// <summary>
        /// Remove all items and name
        /// </summary>
        public void RemoveItems()
        {
            Name.text = "";

            for (int i = 0; i < Items.content.childCount; i++)
                Destroy(Items.content.GetChild(i));
        }
    }
}
