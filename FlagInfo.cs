using System.Collections.Generic;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>Human-readable notes for the game's story flags, shown in the overlay.</summary>
    internal static class FlagInfo
    {
        private const string YarnOncePrefix = "Yarn.Internal.Once.";

        private static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>
        {
            // Level bookkeeping (set by LevelFlow at level start / end)
            { "level_1", "Level 1 started" },
            { "level_1_complete", "Level 1 finished" },
            { "level_5", "Level 5 started" },
            { "level_5_complete", "Level 5 finished" },
            { "level_6_started", "Level 6 started" },
            { "isEditor", "Editor-only flag, always false in builds" },

            // Dragon interaction state
            { "DragonFinishedFromHandJob", "Current dragon was finished by hand" },
            { "DragonOnaholeSexInProgress", "Mount session in progress" },
            { "DragonOnaholeSexCompleted", "Mount session finished" },
            { "DragonDenyMount", "Dragon refused the mount" },

            // Mount / medkit / picnic side objectives
            { "HasMountFrame", "Mount frame is in your possession" },
            { "DeliveredMountFrame", "Mount frame delivered" },
            { "BuiltMount", "Mount assembled" },
            { "PlacedMount", "Mount placed in the wash area" },
            { "PlacedMountForUse", "Mount placed and ready for the dragon" },
            { "MedkitCompleted", "Medkit / bandage objective done" },
            { "PicnicPlaced", "Picnic set up" },
            { "PicnicCompleted", "Picnic objective done" },

            // Dialogue progress
            { "has_talked_to_ryan", "Talked to Ryan on the phone" },
            { "has_asked_question", "Asked the dragon a question (dialogue gate)" },
            { "suggested_ryan_to_conrad", "Suggested Ryan to Conrad" },
            { "conrad_used_mount_3", "Conrad used the mount (visit 3)" },
            { "conrad_jerked_off_3", "Conrad finished by hand (visit 3)" },

            // Romance routes and cutscenes. A *_sex_scene flag without the matching
            // finished_watching_* flag makes the game play that cutscene on next load.
            { "conrad_romanced", "Conrad route unlocked" },
            { "conrad_sex_scene", "Conrad cutscene queued" },
            { "finished_watching_conrad_sex_scene", "Conrad cutscene already watched" },
            { "ryan_romanced", "Ryan route unlocked" },
            { "ryan_sex_scene", "Ryan cutscene queued" },
            { "finished_watching_ryan_sex_scene", "Ryan cutscene already watched" },
            { "ryan_conrad_romanced", "Ryan + Conrad route unlocked" },
            { "ryan_conrad_sex_scene", "Ryan + Conrad cutscene queued" },
            { "finished_watching_conrad_ryan_sex_scene", "Ryan + Conrad cutscene already watched" },
            { "alexander_romanced", "Alexander route unlocked" },
            { "alexander_sex_scene", "Alexander cutscene queued" },
            { "finished_watching_alexander_sex_scene", "Alexander cutscene already watched" },
        };

        /// <summary>Yarn's internal "show this line once" markers; noise for practice purposes.</summary>
        public static bool IsInternal(string id) => id.StartsWith(YarnOncePrefix);

        public static string Describe(string id)
        {
            if (Descriptions.TryGetValue(id, out string text)) return text;
            if (id.EndsWith("_complete")) return "Level finished";
            if (id.EndsWith("_started") || id.StartsWith("level_")) return "Level started";
            return "";
        }
    }
}
