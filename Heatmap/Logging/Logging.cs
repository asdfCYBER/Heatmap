using System.Collections.Generic;
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
    }
}
