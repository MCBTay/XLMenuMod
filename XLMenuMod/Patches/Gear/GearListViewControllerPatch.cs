using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Gear;
using XLMenuMod.Gear.Interfaces;
using XLMenuMod.Interfaces;

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
                            gearListViewItem.Label.fontSize = 36;
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
                if (CustomGearManager.Instance.CurrentFolder != null && CustomGearManager.Instance.CurrentFolder.Children != null && CustomGearManager.Instance.CurrentFolder.Children.Any())
                {
                    __result = GetGear(CustomGearManager.Instance.CurrentFolder.Children);
                }
            }

            static List<ICharacterCustomizationItem> GetGear(List<ICustomInfo> customGear)
            {
                var gear = new List<ICharacterCustomizationItem>();

                foreach (var gearItem in customGear.Select(x => x.GetParentObject()))
                {
                    if (gearItem is CustomGearFolderInfo folderInfo) gear.Add(folderInfo);
                    else if (gearItem is CustomBoardGearInfo boardGear) gear.Add(boardGear);
                    else if (gearItem is CustomCharacterGearInfo charGear) gear.Add(charGear);
                }

                return gear;
            }
        }

        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.OnItemSelected))]
        static class OnItemSelectedPatch
        {
            static bool Prefix(GearListViewController __instance, ref ICharacterCustomizationItem item)
            {
                if (CustomGearManager.Instance.LastSelectedTime != 0d && Time.realtimeSinceStartup - CustomGearManager.Instance.LastSelectedTime < 0.25f) return false;
                CustomGearManager.Instance.LastSelectedTime = Time.realtimeSinceStartup;

                if (item is ICustomGearInfo selectedItem)
                {
                    if (selectedItem.Info is CustomFolderInfo folder)
                    {
                        if (folder.GetName() == "..\\")
                        {
                            CustomGearManager.Instance.CurrentFolder = CustomGearManager.Instance.CurrentFolder.Parent;
                        }
                        else
                        {
                            CustomGearManager.Instance.CurrentFolder = folder;
                        }

                        EventSystem.current.SetSelectedGameObject(null);
                        __instance.UpdateList();
                        CustomGearManager.Instance.UpdateLabel();
                    }
                    else
                    {
                        if (selectedItem is CustomBoardGearInfo boardGear) item = boardGear;
                        else if (selectedItem is CustomCharacterGearInfo charGear) item = charGear;
                    }
                }

                return true;
            }
        }
    }
}
