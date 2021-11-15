using System;
using System.Threading.Tasks;
using System.Timers;
using Game;
using Game.Context;
using Game.Level;
using Game.Mod;
using Game.Railroad;
using Game.Time;
using HarmonyLib;
using Heatmap.UI;
using Utils;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public class Heatmap : AbstractMod
    {
        public static Heatmap Instance { get; private set; }

        internal static IControllers _controller;

        internal static ITimeController _timeController;

        private const string _id = "mod.asdfcyber.heatmap";

        private readonly Harmony _harmony = new Harmony(_id);

        public override CachedLocalizedString Title => "Heatmap";

        public override CachedLocalizedString Description
            => "Colors tracks based on how busy they are";

        private Timer _timer = new Timer(1000); // one second timer for updating colors

        private int _timerIteration = 0; // tracks how often the timer has elapsed

        /// <summary>
        /// true if the current gamemode is <see cref="GameMode.Play"/>, otherwise false.
        /// UI and custom <see cref="BoardNode"/> coloring are turned off when false.
        /// </summary>
        public bool AllowOverlay { get; set; } = false;

        public override async Task OnEnable()
        {
            if (Instance != null)
                Log("Heatmap was already instantiated!", LogLevel.Warning);

            Instance = this;
            Log("Mod has been enabled", LogLevel.Info);

            // Hook into game methods (see Heatmap.Patches)
            try
            {
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log($"Exception during patching: {e.GetType()}, {e.Message}",
                    LogLevel.Exception);
            }

            _timer.Elapsed += RefreshAllNodes;
            await Task.Yield();
        }

        public override async Task OnDisable()
        {
            AllowOverlay = false;
            Log("Mod has been disabled", LogLevel.Info);

            // Remove hooks and reset color
            EventManager.StopListening(EventManager.LevelStarted, InitializeTracker);
            _harmony.UnpatchAll(_id);
            RefreshAllNodes();

            await Task.Yield();
        }
        
        public override async Task OnContextChanged(IControllers controller)
        {
            _controller = controller;
            _timeController = _controller.GameControllers.TimeController;

            if (_controller.CurrentMode == GameMode.Play)
            {
                AllowOverlay = true;
                EventManager.StartListening(EventManager.LevelStarted, InitializeTracker);
                ToolbarButton.Show();
                _timer.Start(); // start auto-refreshing
            }
            else
            {
                AllowOverlay = false;
                EventManager.StopListening(EventManager.LevelStarted, InitializeTracker);
                _timer.Stop(); // stop auto-refreshing
            }

            await Task.Yield();
        }

        private void InitializeTracker(object[] args)
        {
            Level level = (Level)args[0];
            Log($"Registering existing trains for level {level.LevelDefinition.Name}",
                LogLevel.Info);
            NodeTimerTracker.Instance.RegisterExistingTrains();
        }

        /// <summary>
        /// Refresh all BoardNodes so that their color is updated
        /// </summary>
        public void RefreshAllNodes()
        {
            foreach (Node node in _controller.NodeRepository.GetNodes())
            {
                // Not every node has a BoardNode, but there is not a HasBoardNode method
                try
                {
                    node.GetBoardNode().Refresh();
                }
                catch (NullReferenceException) { }
            }
        }

        /// <summary>
        /// Called by a timer so BoardNodes which haven't had their
        /// AllocationState changed recently also change color
        /// </summary>
        private void RefreshAllNodes(object sender, ElapsedEventArgs e)
        {
            if (!_timeController.IsTimeRunning() || !AllowOverlay || !ToolbarButton.Value)
                return;

            _timerIteration++;

            // 25x speed, 15x speed and default: refresh every second,
            // 5x speed: refresh every 3 seconds, 1x speed: refresh every 10 seconds
            int maxIteration = 1;
            if (_timeController.TimeMultiplier == 5)
                maxIteration = 3;
            else if (_timeController.TimeMultiplier == 1)
                maxIteration = 15;

            if (_timerIteration >= maxIteration)
            {
                _timerIteration = 0;
                RefreshAllNodes();
                Log($"Auto-refreshing all nodes at {DateTime.Now:HH:mm:ss} " +
                    $"(in-game: {_timeController.CurrentTime})", LogLevel.Info);
            }
        }
    }
}
