using System;
using System.Threading.Tasks;
using Game;
using Game.Context;
using Game.Level;
using Game.Mod;
using Game.Railroad;
using HarmonyLib;
using Utils;
using static Heatmap.Logging.Logging;

namespace Heatmap
{
    public class Heatmap : AbstractMod
    {
        public static Heatmap Instance { get; private set; }

        internal static IControllers _controller;

        private const string _id = "mod.asdfcyber.heatmap";

        private readonly Harmony _harmony = new Harmony(_id);

        public override CachedLocalizedString Title => "Heatmap";

        public override CachedLocalizedString Description => "Colors tracks based on how busy they are";

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
                Log($"Exception during patching: {e.GetType()}, {e.Message}", LogLevel.Exception);
            }

            await Task.Yield();
        }

        public override async Task OnDisable()
        {
            // TODO: destroy UI components
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

            if (_controller.CurrentMode == GameMode.Play)
            {
                // Attach placeholder UI to some random object
                // TODO: find proper object, and also make a proper UI
                _controller.EventManager.gameObject.AddComponent<UI.HeatmapToggle>();

                AllowOverlay = true;
                EventManager.StartListening(EventManager.LevelStarted, InitializeTracker);
            }
            else
            {
                AllowOverlay = false;
                EventManager.StopListening(EventManager.LevelStarted, InitializeTracker);
            }

            await Task.Yield();
        }

        internal void InitializeTracker(object[] args)
        {
            Level level = (Level)args[0];
            // TODO: load from file, use level.LevelDefinition.Uuid .TotalTime for a unique id
            Log($"Registering existing trains for level {level.LevelDefinition.Name}", LogLevel.Info);
            NodeTimerTracker.Instance.RegisterExistingTrains();
        }

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
    }
}
