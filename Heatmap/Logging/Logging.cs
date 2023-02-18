using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Heatmap.Logging
{
    public static class Logging
    {
        public enum LogLevel
        {
            None,
            Exception,
            Warning,
            Info,
            Debug
        }

        private const string _tag = "[Heatmap] ";

        /// <summary>
        /// Added to messages to help easily find important ones in logs
        /// </summary>
        private static readonly Dictionary<LogLevel, string> _prefix = new Dictionary<LogLevel, string>
        {
            { LogLevel.None, "" },
            { LogLevel.Exception, "EXCEPTION: " },
            { LogLevel.Warning, "WARNING: " },
            { LogLevel.Info, "" },
            { LogLevel.Debug, "" },
        };

        // TODO: include UI to let this be set from the game
        // TODO: include loading/saving from config
        public static LogLevel Level { get; set; } = LogLevel.Info;

        /// <summary>
        /// Log 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level">Must be higher than <see cref="Level"/> to appear in the logs</param>
        public static void Log(string message, LogLevel level)
        {
            if (level <= Level)
                Debug.Log(_tag + _prefix[level] + message);
        }

        /// <summary>
        /// Log a hierarchy of gameobjects with their components 
        /// </summary>
        public static void DumpGameObjectRecursive(GameObject obj, int maxDepth = 2)
        {
            StringBuilder output = new StringBuilder();
            DumpGameObjectRecursive(obj, ref output, maxDepth: maxDepth);

            if (output.Length > 10000)
                output.AppendLine("Safety limit of 10000 characters exceeded");

            Debug.Log(output);
        }

        private static void DumpGameObjectRecursive(GameObject obj, ref StringBuilder output, int maxDepth, int depth = 0)
        {
            output.AppendLine();
            if (output.Length > 10000) return; // Safety limit

            // Print gameobject name
            string objectPadding = new string(' ', depth * 4);
            output.AppendLine($"{objectPadding}gameobject with name {obj.name}");

            // Print the type of every component in the gameobject, indented one level relative to obj
            string componentPadding = new string(' ', depth * 4 + 4);
            foreach (Component component in obj.GetComponents<Component>())
                output.AppendLine($"{componentPadding}component of type {component.GetType().FullName}");

            // Call this function for every child object until maxdepth is reached
            if (depth >= maxDepth) return;
            for (int i = 0; i < obj.transform.childCount; i++)
                DumpGameObjectRecursive(obj.transform.GetChild(i).gameObject, ref output, maxDepth, depth + 1);
        }
    }
}
