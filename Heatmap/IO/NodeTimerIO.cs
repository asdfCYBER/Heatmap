using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static Heatmap.Logging.Logging;

namespace Heatmap.IO
{
    public static class NodeTimerIO
    {
        public const string HeatmapDataFormat = ".heatmapdata";

        /// <summary>
        /// Serialize NodeTimerTracker.NodeTimers as JSON, and
        /// save it in a subfolder of the mod as a .heatmapdata file
        /// </summary>
        /// <param name="saveName">What the save is called (excluding extension)</param>
        /// <param name="tracker">The NodeTimerTracker instance of which to save
        /// the NodeTimers property</param>
        public static void Save(string saveName, NodeTimerTracker tracker)
        {
            saveName += HeatmapDataFormat;

            string directory = Utils.GetSavesDirectory();
            if (!Utils.TryCreateDirectory(directory))
                return;

            string contents = JsonConvert.SerializeObject(tracker.NodeTimers);
            if (Utils.TrySaveToFile(Path.Combine(directory, saveName), contents))
                Log($"Node data saved succesfully as {saveName}", LogLevel.Info);
        }

        /// <summary>
        /// Deserialize saved NodeTimerTracker.NodeTimers data
        /// </summary>
        /// <param name="saveName">The save that should be loaded (excluding extension)</param>
        /// <param name="tracker">The NodeTimerTracker</param>
        public static void Load(string saveName, NodeTimerTracker tracker)
        {
            saveName += HeatmapDataFormat;

            string directory = Utils.GetSavesDirectory();
            if (!Directory.Exists(directory))
            {
                Log($"Failed to load save {saveName} because the {Utils.SaveFolderName} " +
                    "directory does not exist", LogLevel.Warning);
                return;
            }
            
            if (!Utils.TryLoadFromFile(Path.Combine(directory, saveName), out string contents))
                return;

            try
            {
                NodeTimerTracker.Instance.NodeTimers =
                    JsonConvert.DeserializeObject<Dictionary<string, List<NodeTimer>>>(contents);
            }
            catch (JsonSerializationException e)
            {
                Log("Node data could not be loaded from " +
                    $"{saveName}: {e.Message}", LogLevel.Exception);
            }

            Log($"Node data loaded succesfully from {saveName}", LogLevel.Info);
        }

        /// <summary>
        /// Rename an existing .heatmapdata file
        /// </summary>
        /// <param name="oldName">The current name of the file (excluding extension)</param>
        /// <param name="newName">The new name of the file (excluding extension)</param>
        public static void Rename(string oldName, string newName)
        {
            string directory = Utils.GetSavesDirectory();
            string oldPath = Path.Combine(directory, oldName) + HeatmapDataFormat;
            string newPath = Path.Combine(directory, newName) + HeatmapDataFormat;

            if (!File.Exists(oldPath) || File.Exists(newPath))
            {
                Log($"Node data for save {oldName} couldn't be renamed to {newName} because the file " +
                    "doesn't exist or there is already a file with the new name", LogLevel.Warning);
                return;
            }

            if (Utils.TryRenameFile(oldPath, newPath))
                Log($"Renamed {oldPath} as {newPath}", LogLevel.Info);
        }

        /// <summary>
        /// Delete the heatmap data for save <paramref name="saveName"/>
        /// </summary>
        public static void Delete(string saveName)
        {
            string directory = Utils.GetSavesDirectory();
            string path = Path.Combine(directory, saveName) + HeatmapDataFormat;
            
            if (!Directory.Exists(directory))
            {
                Log($"Failed to delete node data for save {saveName} because the " +
                    $"{Utils.SaveFolderName} directory does not exist", LogLevel.Warning);
                return;
            }
            else if (!File.Exists(path))
            {
                Log($"Failed to delete node data for save {saveName} because " +
                    "the file does not exist", LogLevel.Warning);
                return;
            }

            if (Utils.TryDeleteFile(path))
                Log($"Node data for save {saveName} successfully deleted", LogLevel.Info);
        }
    }
}
