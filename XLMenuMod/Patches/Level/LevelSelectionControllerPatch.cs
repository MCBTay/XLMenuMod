using Harmony12;
using System.Collections.Generic;
using System.Linq;
using XLMenuMod.Levels;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Patches.Level
{
    public class LevelSelectionControllerPatch : LevelSelectionController
    {
        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ToggleShowCustom))]
        public static class ToggleShowCustomPatch
        {
            static void Postfix()
            {
                CustomLevelManager.CurrentFolder = null;
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.OnItemSelected))]
        public static class OnItemSelectedPatch
        {
            static bool Prefix(LevelSelectionController __instance, ref LevelInfo level)
            {
                if (level is ICustomLevelInfo)
                {
                    var selectedLevel = level as ICustomLevelInfo;
                    if (selectedLevel == null) return true;

                    if (selectedLevel is CustomFolderInfo)
                    {
                        var selectedFolder = selectedLevel as CustomFolderInfo;
                        if (selectedFolder == null) return true;

                        if (selectedFolder.GetName() == "..\\")
                        {
                            if (selectedFolder.Parent == null)
                            {
                                CustomLevelManager.SetCurrentFolder(null);
                            }
                            else
                            {
                                CustomLevelManager.SetCurrentFolder(selectedFolder.Parent as CustomFolderInfo);
                            }
                        }
                        else
                        {
                            CustomLevelManager.SetCurrentFolder(selectedFolder);
                        }

                        // do this so that OnItemSelected can continue on, and hopefully prevent the double selects
                        level = LevelManager.Instance.currentLevel;
                        return true;
                    }
                    else
                    {
                        CustomLevelManager.SetCurrentFolder(null);
                        return true;
                    }
                }
                else
                {
                    CustomLevelManager.SetCurrentFolder(null);
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.Items), MethodType.Getter)]
        static class ItemsPatch
        {
            static void Postfix(LevelSelectionController __instance, ref List<LevelInfo> __result)
            {
                if (__instance.showCustom)
                {
                    if (CustomLevelManager.CurrentFolder != null && CustomLevelManager.CurrentFolder.Children != null && CustomLevelManager.CurrentFolder.Children.Any())
                    {
                        __result.Clear();

                        foreach (var customLevel in CustomLevelManager.CurrentFolder.Children.OrderBy(x => x.GetName()))
                        {
                            if (customLevel is CustomFolderInfo)
                                __result.Add(customLevel as CustomFolderInfo);
                            else if (customLevel is CustomLevelInfo)
                                __result.Add(customLevel as CustomLevelInfo);
                        }
                    }
                    else
                    {
                        __result.Clear();

                        foreach (var customLevel in CustomLevelManager.NestedCustomLevels.OrderBy(x => x.GetName()))
                        {
                            if (customLevel is CustomFolderInfo)
                                __result.Add(customLevel as CustomFolderInfo);
                            else if (customLevel is CustomLevelInfo)
                                __result.Add(customLevel as CustomLevelInfo);
                        }
                    }
                }
            }
        }
    }
}
