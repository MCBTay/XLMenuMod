using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Gear;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Patches.Gear
{
    public class GearListViewControllerPatch
    {
        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.ItemPrefab), MethodType.Getter)]
        static class ItemPrefabPatch
        {
            static void Postfix(ref ListViewItem<ICharacterCustomizationItem> __result)
            {
                if (__result is GearListViewItem)
                {
                    var gearListViewItem = __result as GearListViewItem;

                    switch (Main.Settings.FontSize)
                    {
                        case FontSizePreset.Small:
                            gearListViewItem.Label.fontSize = 30;
                            break;
                        case FontSizePreset.Smaller:
                            gearListViewItem.Label.fontSize = 24;
                            break;
                        case FontSizePreset.Normal:
                        default:
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.Items), MethodType.Getter)]
        static class ItemsPatch
        {
            static void Postfix(ref List<ICharacterCustomizationItem> __result)
            {
                if (CustomGearManager.CurrentFolder != null &&CustomGearManager.CurrentFolder.Children != null && CustomGearManager.CurrentFolder.Children.Any())
                {
                    __result = GetGear(CustomGearManager.CurrentFolder.Children);
                }
            }

            static List<ICharacterCustomizationItem> GetGear(List<ICustomGearInfo> customGear)
            {
                var gear = new List<ICharacterCustomizationItem>();

                foreach (var gearItem in customGear.OrderBy(x => x.GetName()))
                {
                    if (gearItem is CustomFolderInfo)
                        gear.Add(gearItem as CustomFolderInfo);
                    else if (gearItem is CustomBoardGearInfo)
                        gear.Add(gearItem as CustomBoardGearInfo);
                    else if (gearItem is CustomCharacterGearInfo)
                        gear.Add(gearItem as CustomCharacterGearInfo);
                }

                return gear;
            }
        }

        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.OnItemSelected))]
        static class OnItemSelectedPatch
        {
            static bool Prefix(GearListViewController __instance, ref ICharacterCustomizationItem item)
            {
                if (CustomGearManager.LastSelectedTime != 0 && Time.realtimeSinceStartup - CustomGearManager.LastSelectedTime < 0.25f) return false;
                CustomGearManager.LastSelectedTime = Time.realtimeSinceStartup;

                if (item is ICustomGearInfo)
                {
                    var selectedItem = item as ICustomGearInfo;
                    if (selectedItem == null) return true;

                    if (selectedItem is CustomFolderInfo)
                    {
                        var folder = selectedItem as CustomFolderInfo;
                        if (folder.GetName() == "..\\")
                        {
                            CustomGearManager.CurrentFolder = CustomGearManager.CurrentFolder.Parent as CustomFolderInfo;
                        }
                        else
                        {
                            CustomGearManager.CurrentFolder = folder;
                        }

                        EventSystem.current.SetSelectedGameObject(null);
                        __instance.UpdateList();
                        CustomGearManager.UpdateLabel();
                    }
                    else
                    {
                        if (selectedItem is CustomBoardGearInfo) 
                            item = selectedItem as CustomBoardGearInfo;
                        else if (selectedItem is CustomCharacterGearInfo) 
                            item = selectedItem as CustomCharacterGearInfo;
                    }
                }

                return true;
            }
        }
    }
}
