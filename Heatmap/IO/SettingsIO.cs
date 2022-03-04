using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static Heatmap.Logging.Logging;

namespace Heatmap.IO
{
    public static class SettingsIO
    {
        public const string Filename = "settings.json";

        /// <summary>
        /// Serialize settings as JSON, and save it in a
        /// subfolder of the mod as a .heatmapdata file
        /// </summary>
        /// <param name="settings">The settings instance to save</param>
        public static void Save(Settings settings)
        {
            string directory = Utils.GetAssemblyDirectory();
            string path = Path.Combine(directory, Filename);

            string contents = JsonConvert.SerializeObject(settings, Formatting.Indented);
            if (Utils.TrySaveToFile(path, contents))
                Log($"Settings saved: {contents}", LogLevel.Info);
        }

        /// <summary>
        /// Deserialize saved settings and set it as the <see cref="Settings.Instance"/>
        /// </summary>
        public static void Load()
        {
            string directory = Utils.GetAssemblyDirectory();

            if (!Utils.TryLoadFromFile(Path.Combine(directory, Filename), out string contents))
                return;

            try
            {
                JsonConvert.DeserializeObject<Settings>(contents);
            }
            catch (JsonSerializationException e)
            {
                Log($"Settings could not be loaded: {e.Message}", LogLevel.Exception);
            }

            Log($"Settings loaded: {contents}", LogLevel.Info);
        }
    }
}
