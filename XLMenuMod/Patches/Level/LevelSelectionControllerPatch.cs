using HarmonyLib;
using Newtonsoft.Json;
using System;
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
            static void Postfix(LevelSelectionController __instance)
            {
                CustomLevelManager.CurrentFolder = null;
                CustomLevelManager.SortLabel.gameObject.SetActive(__instance.showCustom);
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.OnItemSelected))]
        public static class OnItemSelectedPatch
        {
            static bool Prefix(LevelSelectionController __instance, ref LevelInfo level)
            {
                if (CustomLevelManager.LastSelectedTime != 0 && Time.realtimeSinceStartup - CustomLevelManager.LastSelectedTime < 0.25f) return false;
                CustomLevelManager.LastSelectedTime = Time.realtimeSinceStartup;

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

                        EventSystem.current.SetSelectedGameObject(null);
                        __instance.UpdateList();
                        CustomLevelManager.UpdateLabel();

                        return false;
                    }
                    else
                    {
                        CustomLevelManager.CurrentFolder = null;
                        CustomLevelManager.UpdateLabel();

                        var customlevel = level as CustomLevelInfo;
                        if (customlevel != null)
                        {
                            var found = LevelManager.Instance.CustomLevels.FirstOrDefault(x => x.hash == customlevel.hash);
                            if (found != null && found is CustomLevelInfo)
                            {
                                (found as CustomLevelInfo).PlayCount++;
                                (found as CustomLevelInfo).LastPlayTime = DateTime.Now;
                            }
                            SaveManager.Instance.SaveCustomLevelListCache(JsonConvert.SerializeObject(LevelManager.Instance.CustomLevels, Formatting.Indented));
                        }

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

                foreach (var customLevel in CustomLevelManager.SortList(customLevels))
                {
                    if (customLevel is CustomFolderInfo)
                        levels.Add(customLevel as CustomFolderInfo);
                    else if (customLevel is CustomLevelInfo)
                        levels.Add(customLevel as CustomLevelInfo);
                }

                return levels;
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), "Awake")]
        public static class AwakePatch
        {
            static void Postfix(LevelSelectionController __instance)
            { 
                CustomLevelManager.CreateSortLabel(__instance);
            }
        }
    }
}
