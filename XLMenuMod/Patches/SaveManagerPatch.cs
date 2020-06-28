using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches
{
    class SaveManagerPatch
    {
        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveCustomLevelListCache))]
        public static class SaveCustomLevelListCachePatch
        {
            static void Postfix(string json)
            {
                File.Copy(SaveManager.Instance.CachedLevelsPath, Path.Combine(Main.ModPath, "CachedCustomLevels.json"), true);
            }
        }

        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.GetCustomLevelFiles))]
        public static class GetCustomLevelFilesCachePatch
        {
            static void Postfix(List<string> __result)
            {
                __result.AddRange(CustomLevelManager.Instance.LoadNestedLevelPaths());
            }
        }
    }
}
