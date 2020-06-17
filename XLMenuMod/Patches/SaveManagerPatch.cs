using HarmonyLib;
using System.IO;

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
    }
}
