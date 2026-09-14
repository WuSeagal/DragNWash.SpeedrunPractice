using System.Collections.Generic;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>
    /// Builds the story-flag set for a jump straight to a level. The set is built
    /// from scratch: carrying flags over from the current save leaves objects from
    /// other levels (picnic, mount...) in the scene, which corrupts the level and has
    /// been observed to crash the renderer.
    /// </summary>
    internal static class LevelJump
    {
        private struct WorldFlag
        {
            public string Id;
            public int AvailableFromLevel;   // 1-based level from which a real run has this flag

            public WorldFlag(string id, int availableFromLevel)
            {
                Id = id;
                AvailableFromLevel = availableFromLevel;
            }
        }

        // Flags set by dialogue (not by LevelFlow) that persist as world state.
        // Levels: 8 = Conrad 3 (mount frame / mount built), 10 = Ryan 4 (picnic).
        private static readonly WorldFlag[] WorldFlags =
        {
            new WorldFlag("has_talked_to_ryan", 5),
            new WorldFlag("HasMountFrame", 9),
            new WorldFlag("DeliveredMountFrame", 9),
            new WorldFlag("BuiltMount", 9),
            new WorldFlag("PlacedMount", 9),
            new WorldFlag("PicnicPlaced", 11),
            new WorldFlag("PicnicCompleted", 11),
        };

        /// <summary>
        /// Flags a real playthrough would have at the start of <paramref name="targetLevel"/>
        /// (0-based). Romance routes and cutscenes are left unset; toggle them in the
        /// overlay's flag list if a route is needed.
        /// </summary>
        public static Dictionary<string, bool> BuildFlagsForLevel(int targetLevel, LevelFlow levelFlow)
        {
            var flags = new Dictionary<string, bool>();
            if (levelFlow == null) return flags;

            int count = levelFlow.GetLevelCount();
            for (int i = 0; i < targetLevel && i < count; i++)
            {
                SetAll(flags, levelFlow.GetSetFlags(i));
                SetAll(flags, levelFlow.GetEndFlags(i));
                string spawnFlag = levelFlow.GetDragonSpawnFlag(i);
                if (!string.IsNullOrEmpty(spawnFlag)) flags[spawnFlag] = true;
            }

            foreach (var world in WorldFlags)
            {
                if (targetLevel + 1 >= world.AvailableFromLevel) flags[world.Id] = true;
            }
            return flags;
        }

        private static void SetAll(Dictionary<string, bool> flags, string[] ids)
        {
            if (ids == null) return;
            foreach (string id in ids)
            {
                if (!string.IsNullOrEmpty(id)) flags[id] = true;
            }
        }
    }
}
