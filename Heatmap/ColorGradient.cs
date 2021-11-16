﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gradient keys are stored in a 'value percentage: Color' format.
        /// A color key for 0% and 100% must be present.
        /// </summary>
        public Dictionary<int, Color> ColorKeys { get; } = new Dictionary<int, Color>();

        /// <summary>
        /// Name of the gradient, necessary for serialization
        /// </summary>
        public string Name { get; }

        private void RegisterGradient(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || Gradients.ContainsKey(name))
                throw new ArgumentException("Gradient name already exists or is invalid");
            else
                Gradients[name] = this;
        }

        /// <summary>
        /// Returns the first key which is >= <paramref name="value"/>,
        /// or 100 if there are no greater values
        /// </summary>
        private int FindClosestLarger(float value)
        {
            int result = (from int key in ColorKeys.Keys
                          where key >= value
                          orderby key
                          select key).FirstOrDefault();
            
            if (result != default) // default(int) is 0
                return result;
            else
                return 100;
        }

        /// <summary>
        /// Returns the first key which is <= <paramref name="value"/>,
        /// or 0 if there are no smaller values
        /// </summary>
        private int FindClosestSmaller(float value)
        {
            int result = (from int key in ColorKeys.Keys
                          where key <= value
                          orderby key descending
                          select key).FirstOrDefault();

            return result; // default(int) is 0
        }

        /// <summary>
        /// Returns a linear interpolation of the two closest values
        /// </summary>
        /// <param name="value">The busyness percentage</param>
        public Color GetColor(float value)
        {
            int greater = FindClosestLarger(value);
            int lower = FindClosestSmaller(value);
            Log($"GetColor value: {value}, greater: {greater}, lower: {lower}", LogLevel.Debug);

            // Avoid DivideByZeroExceptions if the closest larger and smaller values are the same
            if (greater == lower)
                return ColorKeys[greater];

            // Get the difference between lower and value, and convert it to a value between
            // 0 and 1. This gives the location on the gradient between two colors, which is
            // used by Color.Lerp which linearly interpolates the two closest colors
            float relativeValue = (value - lower) / (greater - lower);
            return Color.Lerp(ColorKeys[lower], ColorKeys[greater], relativeValue);
        }

        /// <summary>
        /// Construct a gradient called <paramref name="name"/> by converting every 
        /// key-value pair in <paramref name="colorKeys"/> to an item in <see cref="ColorKeys"/>.
        /// <paramref name="colorKeys"/> must contain at least at least a key for
        /// 0% and for 100%, and the values must be either three (RGB) or four (RGBA) long.
        /// </summary>
        /// <exception cref="ArgumentException">If less than two colors are given
        /// or if the values are of incorrect length</exception>
        public ColorGradient(string name, Dictionary<int, float[]> colorKeys)
        {
            if (!colorKeys.ContainsKey(0) || !colorKeys.ContainsKey(100))
                throw new ArgumentException("A 0% and/or 100% key is missing");

            foreach (KeyValuePair<int, float[]> kvp in colorKeys)
            {
                float[] value = kvp.Value;

                if (value.Length < 3 || value.Length > 4)
                    throw new ArgumentException("Every float[] should " +
                        "have either three (RGB) or four (RGBA) items");
                else if (value.Length == 3)
                    ColorKeys.Add(kvp.Key, new Color(value[0], value[1], value[2]));
                else // == 4
                    ColorKeys.Add(kvp.Key, new Color(value[0], value[1], value[2], value[3]));
            }

            RegisterGradient(name);
            Name = name;
        }

        /// <summary>
        /// Construct a gradient called <paramref name="name"/> by copying every key-value 
        /// pair in <paramref name="colorKeys"/> to <see cref="ColorKeys"/>.
        /// There must be at least two colors, one for 0% and one for 100%.
        /// </summary>
        /// <exception cref="ArgumentException">If less than two colors are given</exception>
        public ColorGradient(string name, Dictionary<int, Color> colorKeys)
        {
            if (!colorKeys.ContainsKey(0) || !colorKeys.ContainsKey(100))
                throw new ArgumentException("A 0% and/or 100% key is missing");

            RegisterGradient(name);
            Name = name;

            foreach (KeyValuePair<int, Color> kvp in colorKeys)
                ColorKeys.Add(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// Construct a gradient called <paramref name="name"/> by spacing <paramref name="colors"/>
        /// evenly on the 0-100% range. At least two colors should be given.
        /// </summary>
        /// <exception cref="ArgumentException">If less than two colors are given</exception>
        public ColorGradient(string name, params Color[] colors)
        {
            if (colors.Length < 2)
                throw new ArgumentException("At least two colors must be given");

            RegisterGradient(name);
            Name = name;

            for (int i = 0; i < colors.Length; i++)
            {
                // Avoid almost-100% being rounded down to 99% by manually rounding
                int key = (int)Math.Round(i * 100 / (colors.Length - 1f));
                ColorKeys.Add(key, colors[i]);
            }
        }
    }
}