using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    public class LevelSelectionControllerPatch
    {
        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ToggleShowCustom))]
        public static class ToggleShowCustomPatch
        {
            static void Postfix(LevelSelectionController __instance)
            {
                CustomLevelManager.Instance.CurrentFolder = null;
                CustomLevelManager.Instance.SortLabel.gameObject.SetActive(__instance.showCustom);
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.OnItemSelected))]
        public static class OnItemSelectedPatch
        {
            static bool Prefix(LevelSelectionController __instance, ref LevelInfo level)
            {
                if (CustomLevelManager.Instance.LastSelectedTime != 0d && Time.realtimeSinceStartup - CustomLevelManager.Instance.LastSelectedTime < 0.25f) return false;
                CustomLevelManager.Instance.LastSelectedTime = Time.realtimeSinceStartup;

                if (level is CustomLevelFolderInfo selectedFolder)
                {
                    if (selectedFolder.FolderInfo.GetName() == "..\\")
                    {
                        CustomLevelManager.Instance.CurrentFolder = selectedFolder.FolderInfo.Parent;
                    }
                    else
                    {
                        CustomLevelManager.Instance.CurrentFolder = selectedFolder.FolderInfo;
                    }

                    EventSystem.current.SetSelectedGameObject(null);
                    __instance.UpdateList();
                    CustomLevelManager.Instance.UpdateLabel();

                    return false;
                }
                else if (level is CustomLevelInfo selectedlevel)
                {
                    level = selectedlevel;
                    return true;
                }
                else
                {
                    CustomLevelManager.Instance.CurrentFolder = null;
                    CustomLevelManager.Instance.UpdateLabel();

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
                            levelListViewItem.LevelNameText.fontSize = 36;
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
                    if (CustomLevelManager.Instance.CurrentFolder != null && CustomLevelManager.Instance.CurrentFolder.Children != null && CustomLevelManager.Instance.CurrentFolder.Children.Any())
                    {
                        __result = GetLevels(CustomLevelManager.Instance.CurrentFolder.Children);
                    }
                    else
                    {
                        __result = GetLevels(CustomLevelManager.Instance.NestedItems);
                    }
                }
            }

            static List<LevelInfo> GetLevels(List<ICustomInfo> customLevels)
            {
                var levels = new List<LevelInfo>();

                foreach (var customLevel in CustomLevelManager.Instance.SortList(customLevels))
                {
                    if (customLevel.IsFolder)
                        levels.Add(customLevel.GetParentObject() as CustomLevelFolderInfo);
                    else
                        levels.Add(customLevel.GetParentObject() as CustomLevelInfo);
                }

                return levels;
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), "Awake")]
        public static class AwakePatch
        {
            static void Postfix(LevelSelectionController __instance)
            {
                CustomLevelManager.Instance.SortLabel = UserInterfaceHelper.CreateSortLabel(__instance.LevelCategoryButton.label, __instance.LevelCategoryButton.transform, ((LevelSortMethod)CustomLevelManager.Instance.CurrentSort).ToString());
            }
        }
    }
}
