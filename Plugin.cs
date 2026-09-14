using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DragNWash.SpeedrunPractice
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "dragnwash.speedrunpractice";
        public const string Name = "DragNWash Speedrun Practice";
        public const string Version = "1.2.0";

        internal static ManualLogSource Log;

        private ConfigEntry<Key> _menuKey;
        private ConfigEntry<Key> _reloadKey;
        private ConfigEntry<Key> _instantCleanKey;
        private ConfigEntry<Key> _skipLevelKey;
        private ConfigEntry<Key> _nextDragonStateKey;

        private bool _menuVisible;
        private int _jumpTarget = 1;
        private float _timeScale = 1f;
        private string _status = "";
        private float _statusUntil;
        private Rect _windowRect = new Rect(20, 20, 520, 640);
        private Vector2 _flagScroll;
        private bool _showFlags;
        private TicketLock.Ticket _cursorTicket;

        private readonly LevelTimer _timer = new LevelTimer();

        private void Awake()
        {
            Log = Logger;

            _menuKey = Config.Bind("Hotkeys", "ToggleMenu", Key.F1, "Show/hide the practice overlay");
            _reloadKey = Config.Bind("Hotkeys", "ReloadLevel", Key.F4, "Restart the current level from its beginning");
            _instantCleanKey = Config.Bind("Hotkeys", "InstantClean", Key.F6, "Instantly clean the active dragon");
            _skipLevelKey = Config.Bind("Hotkeys", "SkipLevel", Key.F7, "Skip the current level");
            _nextDragonStateKey = Config.Bind("Hotkeys", "NextDragonState", Key.F8, "Advance the dragon to its next state");

            Log.LogInfo($"{Name} {Version} loaded. Press {_menuKey.Value} for the practice menu.");
        }

        private void OnDestroy()
        {
            WalkNWashSceneState.dragonStateChanged -= _timer.OnDragonStateChanged;
            ReleaseCursor();
        }

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb[_menuKey.Value].wasPressedThisFrame) ToggleMenu();
            if (kb[_reloadKey.Value].wasPressedThisFrame) ReloadLevel();
            if (kb[_instantCleanKey.Value].wasPressedThisFrame) InstantClean();
            if (kb[_skipLevelKey.Value].wasPressedThisFrame) SkipLevel();
            if (kb[_nextDragonStateKey.Value].wasPressedThisFrame) NextDragonState();

            // The game nulls the dragonStateChanged delegate on every scene load
            // (see WalkNWashSceneState.Init), so re-subscribe whenever we are dropped.
            bool inPlay = GameAccess.InPlayScene;
            if (inPlay && !_timer.Subscribed)
            {
                WalkNWashSceneState.dragonStateChanged += _timer.OnDragonStateChanged;
                _timer.Subscribed = true;
            }
            if (!inPlay) _timer.Subscribed = false;
        }

        // ---- actions -------------------------------------------------------

        private void ToggleMenu()
        {
            _menuVisible = !_menuVisible;
            if (_menuVisible) AcquireCursor(); else ReleaseCursor();
        }

        private void AcquireCursor()
        {
            if (_cursorTicket != null || GameStateManager.Instance == null) return;
            _cursorTicket = GameStateManager.RequestCursorUnlock(this, disablePlayerInput: true);
        }

        private void ReleaseCursor()
        {
            if (_cursorTicket == null) return;
            GameStateManager.ReleaseCursorUnlock(ref _cursorTicket);
            _cursorTicket = null;
        }

        private void ReloadLevel()
        {
            if (!GameAccess.InPlayScene) { SetStatus("Not in a level"); return; }
            // Persist the current level index so the reload starts here, not at the last autosave.
            WalkNWashSceneState.ForceSave();
            GameAccess.ReloadPlayScene();
            SetStatus("Reloading level...");
            CloseMenu();
        }

        private void JumpToLevel(int oneBasedLevel)
        {
            if (!GameAccess.InPlayScene) { SetStatus("Not in a level"); return; }
            int target = oneBasedLevel - 1;
            var flags = GameAccess.GetBoolFlags();
            LevelJump.ApplyFlagsForLevel(target, GameAccess.LevelFlow, flags);
            GameAccess.SaveLevelAndFlags(target, flags);
            GameAccess.ReloadPlayScene();
            SetStatus($"Jumping to level {oneBasedLevel}...");
            CloseMenu();
        }

        private void InstantClean()
        {
            if (!GameAccess.InPlayScene) return;
            SetStatus(GameAccess.TryInstantClean() ? "Dragon cleaned" : "Clean cheat not available");
        }

        private void SkipLevel()
        {
            if (!GameAccess.InPlayScene) return;
            WalkNWashSceneState.SkipLevel();
            SetStatus("Skipping level...");
        }

        /// <summary>
        /// The game only accepts transitions to the immediately following state
        /// (see WalkNWashSceneState.SetDragonState), so we can only step forward.
        /// </summary>
        private void NextDragonState()
        {
            if (!GameAccess.InPlayScene) return;
            var current = WalkNWashSceneState.GetDragonState();
            if (current == WalkNWashSceneState.DragonState.Exited) { SetStatus("Dragon already exited"); return; }
            var next = (WalkNWashSceneState.DragonState)((int)current + 1);
            WalkNWashSceneState.SetDragonState(next);
            SetStatus($"Dragon state: {current} -> {next}");
        }

        private void CloseMenu()
        {
            _menuVisible = false;
            ReleaseCursor();
        }

        private void SetStatus(string text)
        {
            _status = text;
            _statusUntil = Time.unscaledTime + 3f;
            Log.LogInfo(text);
        }

        // ---- overlay -------------------------------------------------------

        private void OnGUI()
        {
            if (!_menuVisible) return;
            _windowRect = GUILayout.Window(GetHashCode(), _windowRect, DrawWindow, $"{Name} [{_menuKey.Value}]");
        }

        private void DrawWindow(int id)
        {
            bool inLevel = GameAccess.InPlayScene;

            GUILayout.Label(inLevel
                ? $"Level {GameAccess.CurrentLevel + 1}/{GameAccess.LevelCount}   Dragon: {GameAccess.DragonName}"
                : "Not in a level (load a save first)");
            if (inLevel)
            {
                GUILayout.Label($"Clean: {WalkNWashSceneState.GetCleanPercentage():P0}   Level time: {_timer.CurrentText}   Last: {_timer.LastText}");
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Dragon state: {WalkNWashSceneState.GetDragonState()}");
                GUILayout.FlexibleSpace();
                if (GUILayout.Button($"Next state > [{_nextDragonStateKey.Value}]")) NextDragonState();
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(8);
            GUILayout.Label("Level select");
            GUILayout.BeginHorizontal();
            int count = GameAccess.LevelCount;
            for (int i = 1; i <= count; i++)
            {
                if (i == 8) { GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(); }
                if (GUILayout.Toggle(i == _jumpTarget, i.ToString(), GUI.skin.button, GUILayout.Width(40))) _jumpTarget = i;
            }
            GUILayout.EndHorizontal();
            GUI.enabled = inLevel;
            if (GUILayout.Button($"Jump to level {_jumpTarget}")) JumpToLevel(_jumpTarget);
            GUI.enabled = true;

            GUILayout.Space(8);
            GUILayout.Label("Cheats");
            GUI.enabled = inLevel;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Reload [{_reloadKey.Value}]")) ReloadLevel();
            if (GUILayout.Button($"Clean [{_instantCleanKey.Value}]")) InstantClean();
            if (GUILayout.Button($"Skip [{_skipLevelKey.Value}]")) SkipLevel();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Weather:", GUILayout.Width(60));
            foreach (WalkNWashSceneState.WeatherState w in Enum.GetValues(typeof(WalkNWashSceneState.WeatherState)))
            {
                if (GUILayout.Button(w.ToString())) WalkNWashSceneState.SetWeatherState(w);
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Time scale: {_timeScale:0.00}x", GUILayout.Width(120));
            float newScale = GUILayout.HorizontalSlider(_timeScale, 0.1f, 2f);
            if (GUILayout.Button("1x", GUILayout.Width(36))) newScale = 1f;
            GUILayout.EndHorizontal();
            if (!Mathf.Approximately(newScale, _timeScale))
            {
                _timeScale = newScale;
                Time.timeScale = _timeScale;
            }

            GUILayout.Space(8);
            _showFlags = GUILayout.Toggle(_showFlags, "Show story flags");
            if (_showFlags && inLevel) DrawFlags();

            if (Time.unscaledTime < _statusUntil) GUILayout.Label(_status);
            GUI.DragWindow();
        }

        /// <summary>
        /// The game's registry only holds flags that were set at least once, so a
        /// fresh save shows a handful. Merge it with the known list so every flag is
        /// always visible; unset ones read as false and get created when toggled.
        /// </summary>
        private void DrawFlags()
        {
            var live = GameAccess.GetBoolFlags();
            _flagScroll = GUILayout.BeginScrollView(_flagScroll, GUILayout.Height(260));

            string group = null;
            foreach (var entry in FlagInfo.Known)
            {
                if (entry.Group != group)
                {
                    group = entry.Group;
                    GUILayout.Space(4);
                    GUILayout.Label($"[{group}]");
                }
                bool exists = live.TryGetValue(entry.Id, out bool value);
                string suffix = exists ? "" : "   (unset)";
                DrawFlagToggle(entry.Id, value, $"{entry.Id}  -  {entry.Description}{suffix}");
            }

            bool otherHeader = false;
            foreach (var kv in live)
            {
                if (FlagInfo.IsKnown(kv.Key) || FlagInfo.IsInternal(kv.Key)) continue;
                if (!otherHeader)
                {
                    otherHeader = true;
                    GUILayout.Space(4);
                    GUILayout.Label("[Other]");
                }
                string note = FlagInfo.Describe(kv.Key);
                DrawFlagToggle(kv.Key, kv.Value, string.IsNullOrEmpty(note) ? kv.Key : $"{kv.Key}  -  {note}");
            }

            GUILayout.EndScrollView();
        }

        private static void DrawFlagToggle(string id, bool value, string label)
        {
            bool next = GUILayout.Toggle(value, label);
            if (next != value) WalkNWashSceneState.SetFlag(id, next);
        }
    }

    /// <summary>Tracks wall-clock time per level using the game's dragon state machine.</summary>
    internal class LevelTimer
    {
        private float _levelStart = -1f;
        private float _lastSplit = -1f;

        public bool Subscribed;

        public void OnDragonStateChanged(WalkNWashSceneState.DragonState state)
        {
            switch (state)
            {
                case WalkNWashSceneState.DragonState.WaitingToAppear:
                    _levelStart = Time.realtimeSinceStartup;
                    break;
                case WalkNWashSceneState.DragonState.Exited:
                    if (_levelStart >= 0f) _lastSplit = Time.realtimeSinceStartup - _levelStart;
                    _levelStart = -1f;
                    break;
            }
        }

        public string CurrentText => _levelStart < 0f ? "--:--.--" : Format(Time.realtimeSinceStartup - _levelStart);
        public string LastText => _lastSplit < 0f ? "--:--.--" : Format(_lastSplit);

        private static string Format(float seconds) => TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss\.ff");
    }
}
