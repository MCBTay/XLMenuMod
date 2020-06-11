using Harmony12;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Levels;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Patches.Level
{
    public class LevelSelectionControllerPatch
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
                            CustomLevelManager.CurrentFolder = selectedFolder.Parent as CustomFolderInfo;
                        }
                        else
                        {
                            CustomLevelManager.CurrentFolder = selectedFolder;
                        }

                        if (CustomLevelManager.CurrentFolder == null)
                        {
                            __instance.LevelCategoryButton.label.text = __instance.showCustom ? "Custom Maps" : "Official Maps";
                        }
                        else
                        {
                            __instance.LevelCategoryButton.label.text = CustomLevelManager.CurrentFolder.GetName();
                        }


                        EventSystem.current.SetSelectedGameObject(null);
                        __instance.UpdateList(); 
                        Event.current.Use();

                        return false;
                    }
                    else
                    {
                        CustomLevelManager.CurrentFolder = null;
                        return true;
                    }
                }
                else
                {
                    CustomLevelManager.CurrentFolder = null;
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ItemPrefab), MethodType.Getter)]
        public static class ItemPrefabPatch
        {
            static void Postfix(ref ListViewItem<LevelInfo> __result)
            {
                if (__result is LevelListItem)
                {
                    var levelListViewItem = __result as LevelListItem;

                    switch (Main.Settings.FontSize)
                    {
                        case FontSizePreset.Small:
                            levelListViewItem.LevelNameText.fontSize = 30;
                            break;
                        case FontSizePreset.Smaller:
                            levelListViewItem.LevelNameText.fontSize = 24;
                            break;
                        case FontSizePreset.Normal:
                        default:
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.Items), MethodType.Getter)]
        public static class ItemsPatch
        {
            static void Postfix(LevelSelectionController __instance, ref List<LevelInfo> __result)
            {
                if (__instance.showCustom)
                {
                    if (CustomLevelManager.CurrentFolder != null && CustomLevelManager.CurrentFolder.Children != null && CustomLevelManager.CurrentFolder.Children.Any())
                    {
                        __result = GetLevels(CustomLevelManager.CurrentFolder.Children);
                    }
                    else
                    {
                        __result = GetLevels(CustomLevelManager.NestedCustomLevels);
                    }
                }
            }

            static List<LevelInfo> GetLevels(List<ICustomLevelInfo> customLevels)
            {
                var levels = new List<LevelInfo>();

                foreach (var customLevel in customLevels.OrderBy(x => x.GetName()))
                {
                    if (customLevel is CustomFolderInfo)
                        levels.Add(customLevel as CustomFolderInfo);
                    else if (customLevel is CustomLevelInfo)
                        levels.Add(customLevel as CustomLevelInfo);
                }

                return levels;
            }
        }


    }
}
