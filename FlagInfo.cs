using System.Collections.Generic;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>Human-readable notes for the game's story flags, shown in the overlay.</summary>
    internal static class FlagInfo
    {
        public struct Entry
        {
            public string Group;
            public string Id;
            public string Description;

            public Entry(string group, string id, string description)
            {
                Group = group;
                Id = id;
                Description = description;
            }
        }

        private const string YarnOncePrefix = "Yarn.Internal.Once.";

        /// <summary>
        /// Every flag the game is known to use, in display order. The live registry
        /// only contains flags that have been set at least once, so the overlay
        /// merges this list with it to always show the full picture.
        /// </summary>
        public static readonly Entry[] Known =
        {
            // Level bookkeeping (set by LevelFlow at level start / end)
            new Entry("Level milestones", "level_1", "Level 1 started"),
            new Entry("Level milestones", "level_1_complete", "Level 1 finished"),
            new Entry("Level milestones", "level_5", "Level 5 started"),
            new Entry("Level milestones", "level_5_complete", "Level 5 finished"),
            new Entry("Level milestones", "level_6_started", "Level 6 started"),

            // Dragon interaction state (reset every level)
            new Entry("Current dragon", "DragonFinishedFromHandJob", "Current dragon was finished by hand"),
            new Entry("Current dragon", "DragonOnaholeSexInProgress", "Mount session in progress"),
            new Entry("Current dragon", "DragonOnaholeSexCompleted", "Mount session finished"),
            new Entry("Current dragon", "DragonDenyMount", "Dragon refused the mount"),

            // Mount / medkit / picnic side objectives
            new Entry("Side objectives", "HasMountFrame", "Mount frame is in your possession"),
            new Entry("Side objectives", "DeliveredMountFrame", "Mount frame delivered"),
            new Entry("Side objectives", "BuiltMount", "Mount assembled"),
            new Entry("Side objectives", "PlacedMount", "Mount placed in the wash area"),
            new Entry("Side objectives", "PlacedMountForUse", "Mount placed and ready for the dragon"),
            new Entry("Side objectives", "MedkitCompleted", "Medkit / bandage objective done"),
            new Entry("Side objectives", "PicnicPlaced", "Picnic set up"),
            new Entry("Side objectives", "PicnicCompleted", "Picnic objective done"),

            // Dialogue progress
            new Entry("Dialogue", "has_talked_to_ryan", "Talked to Ryan on the phone"),
            new Entry("Dialogue", "has_asked_question", "Asked the dragon a question (dialogue gate)"),
            new Entry("Dialogue", "suggested_ryan_to_conrad", "Suggested Ryan to Conrad"),
            new Entry("Dialogue", "conrad_used_mount_3", "Conrad used the mount (visit 3)"),
            new Entry("Dialogue", "conrad_jerked_off_3", "Conrad finished by hand (visit 3)"),

            // Romance routes and cutscenes. A *_sex_scene flag without the matching
            // finished_watching_* flag makes the game play that cutscene on next load.
            new Entry("Romance / cutscenes", "conrad_romanced", "Conrad route unlocked"),
            new Entry("Romance / cutscenes", "conrad_sex_scene", "Conrad cutscene queued"),
            new Entry("Romance / cutscenes", "finished_watching_conrad_sex_scene", "Conrad cutscene already watched"),
            new Entry("Romance / cutscenes", "ryan_romanced", "Ryan route unlocked"),
            new Entry("Romance / cutscenes", "ryan_sex_scene", "Ryan cutscene queued"),
            new Entry("Romance / cutscenes", "finished_watching_ryan_sex_scene", "Ryan cutscene already watched"),
            new Entry("Romance / cutscenes", "ryan_conrad_romanced", "Ryan + Conrad route unlocked"),
            new Entry("Romance / cutscenes", "ryan_conrad_sex_scene", "Ryan + Conrad cutscene queued"),
            new Entry("Romance / cutscenes", "finished_watching_conrad_ryan_sex_scene", "Ryan + Conrad cutscene already watched"),
            new Entry("Romance / cutscenes", "alexander_romanced", "Alexander route unlocked"),
            new Entry("Romance / cutscenes", "alexander_sex_scene", "Alexander cutscene queued"),
            new Entry("Romance / cutscenes", "finished_watching_alexander_sex_scene", "Alexander cutscene already watched"),
        };

        private static readonly HashSet<string> KnownIds = BuildKnownIds();

        private static HashSet<string> BuildKnownIds()
        {
            var set = new HashSet<string>();
            foreach (var e in Known) set.Add(e.Id);
            return set;
        }

        public static bool IsKnown(string id) => KnownIds.Contains(id);

        /// <summary>Yarn's internal "show this line once" markers, plus the editor-only flag; noise for practice purposes.</summary>
        public static bool IsInternal(string id) => id.StartsWith(YarnOncePrefix) || id == "isEditor";

        public static string Describe(string id)
        {
            foreach (var e in Known)
            {
                if (e.Id == id) return e.Description;
            }
            if (id.EndsWith("_complete")) return "Level finished";
            if (id.EndsWith("_started") || id.StartsWith("level_")) return "Level started";
            return "";
        }
    }
}
