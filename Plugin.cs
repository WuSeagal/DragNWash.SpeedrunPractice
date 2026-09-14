using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DragNWash.SpeedrunPractice
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Guid = "dragnwash.speedrunpractice";
        public const string Name = "DragNWash Speedrun Practice";
        public const string Version = "1.0.0";

        private const int StateSlotCount = 5;

        internal static ManualLogSource Log;

        private ConfigEntry<Key> _menuKey;
        private ConfigEntry<Key> _saveStateKey;
        private ConfigEntry<Key> _loadStateKey;
        private ConfigEntry<Key> _reloadKey;
        private ConfigEntry<Key> _instantCleanKey;
        private ConfigEntry<Key> _skipLevelKey;
        private ConfigEntry<Key> _prevSlotKey;
        private ConfigEntry<Key> _nextSlotKey;

        private bool _menuVisible;
        private int _activeSlot = 1;
        private int _jumpTarget = 1;
        private float _timeScale = 1f;
        private string _status = "";
        private float _statusUntil;
        private Rect _windowRect = new Rect(20, 20, 420, 560);
        private Vector2 _flagScroll;
        private bool _showFlags;
        private TicketLock.Ticket _cursorTicket;

        private readonly LevelTimer _timer = new LevelTimer();

        private string StateDir => Path.Combine(Paths.ConfigPath, "DragNWash.SpeedrunPractice");

        private void Awake()
        {
            Log = Logger;

            _menuKey = Config.Bind("Hotkeys", "ToggleMenu", Key.F1, "Show/hide the practice overlay");
            _saveStateKey = Config.Bind("Hotkeys", "SaveState", Key.F5, "Save current level+flags into the active slot");
            _loadStateKey = Config.Bind("Hotkeys", "LoadState", Key.F9, "Load the active slot and reload the level");
            _reloadKey = Config.Bind("Hotkeys", "ReloadLevel", Key.F4, "Restart the current level from its beginning");
            _instantCleanKey = Config.Bind("Hotkeys", "InstantClean", Key.F6, "Instantly clean the active dragon");
            _skipLevelKey = Config.Bind("Hotkeys", "SkipLevel", Key.F7, "Skip the current level");
            _prevSlotKey = Config.Bind("Hotkeys", "PrevSlot", Key.PageDown, "Select previous state slot");
            _nextSlotKey = Config.Bind("Hotkeys", "NextSlot", Key.PageUp, "Select next state slot");

            Directory.CreateDirectory(StateDir);
            Harmony.CreateAndPatchAll(typeof(Patches), Guid);

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
            if (kb[_saveStateKey.Value].wasPressedThisFrame) SaveState(_activeSlot);
            if (kb[_loadStateKey.Value].wasPressedThisFrame) LoadState(_activeSlot);
            if (kb[_reloadKey.Value].wasPressedThisFrame) ReloadLevel();
            if (kb[_instantCleanKey.Value].wasPressedThisFrame) InstantClean();
            if (kb[_skipLevelKey.Value].wasPressedThisFrame) SkipLevel();
            if (kb[_prevSlotKey.Value].wasPressedThisFrame) SelectSlot(_activeSlot - 1);
            if (kb[_nextSlotKey.Value].wasPressedThisFrame) SelectSlot(_activeSlot + 1);

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

        private void SelectSlot(int slot)
        {
            _activeSlot = Mathf.Clamp(slot, 1, StateSlotCount);
            SetStatus($"Slot {_activeSlot} selected");
        }

        private string SlotPath(int slot) => Path.Combine(StateDir, $"state_{slot}.json");

        private void SaveState(int slot)
        {
            if (!GameAccess.InPlayScene) { SetStatus("Not in a level"); return; }
            try
            {
                File.WriteAllText(SlotPath(slot), GameAccess.SerializeCurrentState());
                SetStatus($"Saved slot {slot} (level {GameAccess.CurrentLevel + 1})");
            }
            catch (Exception e)
            {
                Log.LogError(e);
                SetStatus("Save failed, see log");
            }
        }

        private void LoadState(int slot)
        {
            if (!GameAccess.InPlayScene) { SetStatus("Not in a level"); return; }
            string path = SlotPath(slot);
            if (!File.Exists(path)) { SetStatus($"Slot {slot} is empty"); return; }
            try
            {
                GameAccess.WriteSaveAndReload(File.ReadAllText(path));
                SetStatus($"Loading slot {slot}...");
                CloseMenu();
            }
            catch (Exception e)
            {
                Log.LogError(e);
                SetStatus("Load failed, see log");
            }
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
            GameAccess.WriteSaveAndReload(GameAccess.BuildSaveJson(target, flags));
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
                GUILayout.Label($"State: {WalkNWashSceneState.GetDragonState()}   Clean: {WalkNWashSceneState.GetCleanPercentage():P0}");
                GUILayout.Label($"Level time: {_timer.CurrentText}   Last: {_timer.LastText}");
            }

            GUILayout.Space(8);
            GUILayout.Label("Practice state slots");
            GUILayout.BeginHorizontal();
            for (int i = 1; i <= StateSlotCount; i++)
            {
                bool exists = File.Exists(SlotPath(i));
                string label = (i == _activeSlot ? "> " : "") + i + (exists ? "*" : "");
                if (GUILayout.Toggle(i == _activeSlot, label, GUI.skin.button)) _activeSlot = i;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.enabled = inLevel;
            if (GUILayout.Button($"Save [{_saveStateKey.Value}]")) SaveState(_activeSlot);
            if (GUILayout.Button($"Load [{_loadStateKey.Value}]")) LoadState(_activeSlot);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

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
            if (_showFlags && inLevel)
            {
                _flagScroll = GUILayout.BeginScrollView(_flagScroll, GUILayout.Height(140));
                foreach (var kv in GameAccess.GetBoolFlags())
                {
                    bool v = GUILayout.Toggle(kv.Value, kv.Key);
                    if (v != kv.Value) WalkNWashSceneState.SetFlag(kv.Key, v);
                }
                GUILayout.EndScrollView();
            }

            if (Time.unscaledTime < _statusUntil) GUILayout.Label(_status);
            GUI.DragWindow();
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
