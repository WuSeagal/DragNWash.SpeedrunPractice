using System.Collections.Generic;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>
    /// Decides what the story flags should look like when the player jumps
    /// straight to <paramref name="targetLevel"/> from the practice menu.
    /// </summary>
    internal static class LevelJump
    {
        // Flags that WalkNWashSceneState.TryCheckForSexSceneNeeded() checks on scene load.
        // Marking the "finished_watching_*" side as true keeps a level jump from
        // replaying a cutscene the player would already have seen in a real run.
        private static readonly string[][] CutscenePairs =
        {
            new[] { "ryan_conrad_sex_scene", "finished_watching_conrad_ryan_sex_scene" },
            new[] { "conrad_sex_scene", "finished_watching_conrad_sex_scene" },
            new[] { "ryan_sex_scene", "finished_watching_ryan_sex_scene" },
            new[] { "alexander_sex_scene", "finished_watching_alexander_sex_scene" },
        };

        /// <summary>
        /// Rebuild <paramref name="flags"/> so it matches what a real playthrough
        /// would have after finishing every level before <paramref name="targetLevel"/>:
        /// start/end flags of earlier levels are set, later levels' flags are cleared.
        /// Levels are 0-based.
        /// </summary>
        public static void ApplyFlagsForLevel(int targetLevel, LevelFlow levelFlow, Dictionary<string, bool> flags)
        {
            if (levelFlow == null) return;

            int count = levelFlow.GetLevelCount();
            for (int i = 0; i < count; i++)
            {
                bool completed = i < targetLevel;
                SetAll(flags, levelFlow.GetSetFlags(i), completed);
                SetAll(flags, levelFlow.GetEndFlags(i), completed);
            }

            foreach (var pair in CutscenePairs)
            {
                if (flags.TryGetValue(pair[0], out bool triggered) && triggered) flags[pair[1]] = true;
            }
        }

        private static void SetAll(Dictionary<string, bool> flags, string[] ids, bool value)
        {
            if (ids == null) return;
            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id)) flags[id] = value;
            }
        }
    }
}
