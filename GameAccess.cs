using System;
using System.Collections.Generic;
using HarmonyLib;
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

        /// <summary>
        /// Persist a level index + flag set using the game's own save path, so this
        /// keeps working when the game changes its save format/location (it did in
        /// build 25286774: SaveManager -> SaveManagerV1 with a new folder layout).
        /// </summary>
        public static void SaveLevelAndFlags(int levelIndex, Dictionary<string, bool> flags)
        {
            Flags.ClearAll();
            foreach (var kv in flags) Flags.Set(kv.Key, kv.Value);
            WalkNWashSceneState.SetLevel(levelIndex);
            WalkNWashSceneState.ForceSave();
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

    }
}
