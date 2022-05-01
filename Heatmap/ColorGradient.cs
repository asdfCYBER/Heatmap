using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public partial class ColorGradient
    {
        /// <summary>
        /// Stores all gradients by name
        /// </summary>
        public static Dictionary<string, ColorGradient> Gradients { get; }
            = new Dictionary<string, ColorGradient>();

        public Gradient Gradient { get; private set; } = new Gradient();

        /// <summary>
        /// Name of the gradient, necessary for serialization
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether the gradient can be edited
        /// </summary>
        public bool Editable { get; }

        private void RegisterGradient(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || Gradients.ContainsKey(name))
                throw new ArgumentException($"Gradient name {name} already exists or is invalid");
            else
                Gradients[name] = this;

            StringBuilder gradientText = new StringBuilder();
            foreach (GradientColorKey key in Gradient.colorKeys)
                gradientText.Append($"\n{key.time}: {key.color}");

            Log($"Registered gradient {name}: {gradientText}", LogLevel.Info);
        }

        /// <summary>
        /// Returns a linear interpolation of the two closest values
        /// </summary>
        /// <param name="value">The busyness percentage</param>
        public Color GetColor(float value) => Gradient.Evaluate(value);

        /// <summary>
        /// Construct a gradient called <paramref name="name"/> by spacing <paramref name="colors"/>
        /// evenly on the 0-100% range. At least two colors should be given.
        /// </summary>
        /// <exception cref="ArgumentException">If less than two colors are given</exception>
        public ColorGradient(string name, bool editable, params Color[] colors)
        {
            if (colors.Length < 2)
                throw new ArgumentException("At least two colors must be given");

            Name = name;
            Editable = editable;

            // For some reason creating an array and filling in the values later doesn't work
            List<GradientColorKey> colorKeys = new List<GradientColorKey>(colors.Length);
            for (int i = 0; i < colors.Length; i++)
            {
                float key = (float)i / (colors.Length - 1);
                colorKeys.Add(new GradientColorKey(colors[i], key));
            }
            Gradient.colorKeys = colorKeys.ToArray();
            
            RegisterGradient(name);
        }

        /// <summary>
        /// Convert a <see cref="UnityEngine.Gradient"/> to a <see cref="ColorGradient"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="editable"></param>
        /// <param name="gradient"></param>
        public ColorGradient(string name, bool editable, Gradient gradient)
        {
            Name = name;
            Editable = editable;
            Gradient = gradient;
            RegisterGradient(name);
        }

        /// <summary>
        /// Delete the gradient from the list of options and the collection of gradients
        /// </summary>
        public void Delete()
        {
            if (!Editable)
                return;
        
            Gradients.Remove(Name);
        }

        /// <summary>
        /// Add this gradient to the UI list of options
        /// </summary>
        public void Save()
        {
            if (!Editable)
                return;
        }
    }
}
