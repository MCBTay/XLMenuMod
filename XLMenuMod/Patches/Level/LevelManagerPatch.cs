using Harmony12;
using System.Collections.Generic;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    public class LevelManagerPatch
    {
        [HarmonyPatch(typeof(LevelManager), "GetCustomLevelFiles")]
        public static class GetCustomLevelFilesPatch
        {
            static void Postfix(ref List<string> __result)
            {
                __result.AddRange(CustomLevelManager.LoadNestedLevelPaths());
            }
        }
    }
}
