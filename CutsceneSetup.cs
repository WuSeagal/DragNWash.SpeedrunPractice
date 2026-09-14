namespace DragNWash.SpeedrunPractice
{
    /// <summary>
    /// Which level's outro dialogue queues each cutscene, and the romance flag that
    /// outro checks. Derived from the LevelConfiguration assets and Yarn scripts
    /// (game build 25286774): the outro sets X_sex_scene only when X_romanced is true,
    /// and the cutscene then plays on the next level load.
    /// </summary>
    internal struct CutsceneSetup
    {
        public string Label;
        public string Intent;          // MenuEventUserIntent handled by Menu.OnEvent
        public int Level;              // 1-based level whose outro queues the cutscene
        public string RomanceFlag;     // must be true for the outro to queue it
        public string SceneFlag;       // X_sex_scene, set by the outro dialogue
        public string WatchedFlag;     // finished_watching_X, set when the cutscene ends

        public static readonly CutsceneSetup[] All =
        {
            new CutsceneSetup { Label = "Ryan+Conrad", Intent = "ConradRyanSexScene", Level = 11, RomanceFlag = "ryan_conrad_romanced", SceneFlag = "ryan_conrad_sex_scene", WatchedFlag = "finished_watching_conrad_ryan_sex_scene" },
            new CutsceneSetup { Label = "Ryan",        Intent = "RyanSexScene",       Level = 13, RomanceFlag = "ryan_romanced",        SceneFlag = "ryan_sex_scene",        WatchedFlag = "finished_watching_ryan_sex_scene" },
            new CutsceneSetup { Label = "Conrad",      Intent = "ConradSexScene",     Level = 14, RomanceFlag = "conrad_romanced",      SceneFlag = "conrad_sex_scene",      WatchedFlag = "finished_watching_conrad_sex_scene" },
            new CutsceneSetup { Label = "Alexander",   Intent = "AlexanderSexScene",  Level = 15, RomanceFlag = "alexander_romanced",   SceneFlag = "alexander_sex_scene",   WatchedFlag = "finished_watching_alexander_sex_scene" },
        };

    }
}
