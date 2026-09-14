using HarmonyLib;
using UnityEngine.UI;

namespace DragNWash.SpeedrunPractice
{
    /// <summary>
    /// The game ships three debug buttons that are hidden behind
    /// <c>Debug.isDebugBuild || Application.isEditor</c>. These postfixes run
    /// right after each cheat's OnEnable and simply turn the button back on.
    /// </summary>
    internal static class Patches
    {
        [HarmonyPatch(typeof(InstantCleanDragonCheat), "OnEnable")]
        [HarmonyPostfix]
        private static void ShowInstantClean(Button ___button)
        {
            if (___button != null) ___button.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(InstantSkipLevelCheat), "OnEnable")]
        [HarmonyPostfix]
        private static void ShowInstantSkip(Button ___button)
        {
            if (___button != null) ___button.gameObject.SetActive(true);
        }

        [HarmonyPatch(typeof(DeleteSaveCheat), "OnEnable")]
        [HarmonyPostfix]
        private static void ShowDeleteSave(Button ___button)
        {
            if (___button != null) ___button.gameObject.SetActive(true);
        }
    }
}
