using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using SimpleJSON;
using UnityEngine;
using UnityEngine.SceneManagement;
using WalkNWash.Flags;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>
    /// Thin wrapper over the game's private state. Everything here is reflection
    /// resolved once via Harmony's AccessTools so per-frame reads stay cheap.
    /// </summary>
    internal static class GameAccess
    {
        private static readonly AccessTools.FieldRef<WalkNWashSceneState> SceneStateInstance =
            AccessTools.StaticFieldRefAccess<WalkNWashSceneState>(AccessTools.Field(typeof(WalkNWashSceneState), "instance"));

        private static readonly AccessTools.FieldRef<WalkNWashSceneState, int> CurrentLevelRef =
            AccessTools.FieldRefAccess<WalkNWashSceneState, int>("currentLevel");

        private static readonly AccessTools.FieldRef<WalkNWashSceneState, LevelFlow> LevelFlowRef =
            AccessTools.FieldRefAccess<WalkNWashSceneState, LevelFlow>("levelFlow");

        private static readonly System.Reflection.PropertyInfo MenuManagerInstance =
            AccessTools.Property(typeof(MenuManager), "instance");

        private static readonly System.Reflection.MethodInfo MenuManagerProcessTransition =
            AccessTools.Method(typeof(MenuManager), "ProcessTransition");

        private static readonly System.Reflection.MethodInfo InstantCleanOnClicked =
            AccessTools.Method(typeof(InstantCleanDragonCheat), "OnClicked");

        public const string PlaySceneName = "PlayGame";
        public const int FallbackLevelCount = 14;

        public static WalkNWashSceneState SceneState => SceneStateInstance();

        public static bool InPlayScene => SceneState != null && SceneManager.GetActiveScene().name == PlaySceneName;

        public static int CurrentLevel
        {
            get
            {
                var state = SceneState;
                return state == null ? -1 : CurrentLevelRef(state);
            }
        }

        public static LevelFlow LevelFlow
        {
            get
            {
                var state = SceneState;
                return state == null ? null : LevelFlowRef(state);
            }
        }

        public static int LevelCount
        {
            get
            {
                var flow = LevelFlow;
                return flow == null ? FallbackLevelCount : flow.GetLevelCount();
            }
        }

        public static string DragonName
        {
            get
            {
                if (!WalkNWashSceneState.TryGetActiveDragon(out var dragon)) return "-";
                return string.IsNullOrEmpty(dragon.name) ? "?" : dragon.name;
            }
        }

        /// <summary>Snapshot of the live flag registry as JSON text (same format as savegame.dgn).</summary>
        public static string SerializeCurrentState()
        {
            return Flags.ToJson(CurrentLevel).ToString();
        }

        /// <summary>Overwrite the active save slot with the given JSON and reload the play scene.</summary>
        public static void WriteSaveAndReload(string json)
        {
            JSONNode node = JSON.Parse(json);
            if (node == null) throw new InvalidDataException("Save JSON failed to parse.");
            SaveManager.Save(node);
            ReloadPlayScene();
        }

        /// <summary>
        /// Route through the game's own menu system so the loading screen, fade and
        /// SceneDescriptor bookkeeping all behave exactly as a normal "Continue".
        /// </summary>
        public static void ReloadPlayScene()
        {
            object manager = MenuManagerInstance.GetValue(null);
            var transition = new MenuResponseTransition(
                "Menu_Loading",
                "Speedrun practice reload",
                new MenuTransitionDataLevelLoad(PlaySceneName));
            MenuManagerProcessTransition.Invoke(manager, new object[] { transition });
        }

        /// <summary>Same as pressing the game's hidden "instant clean" debug button.</summary>
        public static bool TryInstantClean()
        {
            var cheat = UnityEngine.Object.FindFirstObjectByType<InstantCleanDragonCheat>(FindObjectsInactive.Include);
            if (cheat == null) return false;
            InstantCleanOnClicked.Invoke(cheat, null);
            return true;
        }

        public static Dictionary<string, bool> GetBoolFlags()
        {
            var result = new Dictionary<string, bool>();
            var all = Flags.GetAllFlags();
            if (all == null) return result;
            foreach (var entry in all)
            {
                if (entry.type == FlagEntry.Type.BOOL) result[entry.id] = entry.boolValue;
            }
            return result;
        }

        /// <summary>Build savegame JSON from a level index and a bool flag set.</summary>
        public static string BuildSaveJson(int levelIndex, Dictionary<string, bool> flags)
        {
            var array = new JSONArray();
            var header = new JSONObject();
            header["levelIndex"] = levelIndex;
            array.Add(header);
            foreach (var kv in flags)
            {
                var obj = new JSONObject();
                obj["id"] = kv.Key;
                obj["type"] = "BOOL";
                obj["boolValue"] = kv.Value;
                obj["stringValue"] = kv.Value;
                array.Add(obj);
            }
            return array.ToString();
        }
    }
}
