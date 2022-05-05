using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static Heatmap.Logging.Logging;

namespace Heatmap.IO
{
    public static class ColorGradientIO
    {
        public const string Filename = "custom_gradients.json";

        /// <summary>
        /// Serialize ColorGradients as JSON and write to gradients.json
        /// </summary>
        /// <param name="gradients">The gradients to save</param>
        public static void Save()
        {
            string directory = Utils.GetAssemblyDirectory();
            string path = Path.Combine(directory, Filename);

            // Serialize all editable gradients
            JArray root = new JArray();
            foreach (ColorGradient gradient in ColorGradient.Gradients.Values)
            {
                if (!gradient.Editable)
                    continue;

                JObject colorKeys = new JObject();
                foreach (var color in gradient.Gradient.colorKeys)
                    colorKeys.Add(color.time.ToString(), "#" + ColorUtility.ToHtmlStringRGB(color.color));

                root.Add(new JObject(
                    new JProperty("name", gradient.Name),
                    new JProperty("colors", colorKeys)
                ));
            }

            string contents = root.ToString(Formatting.Indented);
            if (Utils.TrySaveToFile(path, contents))
                Log($"Gradients saved: {contents}", LogLevel.Info);
        }

        /// <summary>
        /// Deserialize gradients.json to <see cref="ColorGradient.Gradients"/>
        /// </summary>
        public static void Load()
        {
            string directory = Utils.GetAssemblyDirectory();
            string path = Path.Combine(directory, Filename);

            // Get JSON string from file
            if (!Utils.TryLoadFromFile(path, out string json))
            {
                Log($"Unable to load custom gradients from {Filename}", LogLevel.Exception);
                return;
            }

            try
            {
                // Iterate over all gradients in the file
                JArray root = JArray.Parse(json);
                foreach (JObject jsonGradient in root.Children<JObject>())
                {
                    // Get the colorKeys and parse them as GradientColorKeys
                    List<GradientColorKey> colorKeys = new List<GradientColorKey>();
                    foreach (JProperty keyValuePair in ((JObject)jsonGradient["colors"]).Properties())
                    {
                        if (!ColorUtility.TryParseHtmlString((string)keyValuePair.Value, out Color color))
                            throw new ArgumentException($"Unable to parse color in gradient {jsonGradient["name"]}");
                        colorKeys.Add(new GradientColorKey(color, float.Parse(keyValuePair.Name)));
                    }

                    // Create the ColorGradient object
                    Gradient gradient = new Gradient { colorKeys = colorKeys.ToArray() };
                    new ColorGradient((string)jsonGradient["name"], true, gradient);
                }
            }
            catch (Exception e) when (
                e is JsonReaderException
                || e is InvalidCastException
                || e is KeyNotFoundException
                || e is ArgumentException
            )
            {
                Log($"Error parsing {Filename}. Error message: {e.Message}", LogLevel.Exception);
                return;
            }
        }
    }
}
