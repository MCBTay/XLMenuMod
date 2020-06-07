using Harmony12;
using System.Collections.Generic;
using System.Linq;
using XLMenuMod.Gear;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Patches.Gear
{
    public class GearListViewControllerPatch
    {
        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.Items), MethodType.Getter)]
        static class ItemsPatch
        {
            static void Postfix(GearListViewController __instance, ref List<ICharacterCustomizationItem> __result)
            {
                if (CustomGearManager.CurrentFolder != null &&
                    CustomGearManager.CurrentFolder.Children != null && 
                    CustomGearManager.CurrentFolder.Children.Any())
                {
                    __result.Clear();
                    __result.AddRange(CustomGearManager.CurrentFolder.Children.OrderBy(x => x.GetName()));
                }
            }
        }

        [HarmonyPatch(typeof(GearListViewController), nameof(GearListViewController.OnItemSelected))]
        static class OnItemSelectedPatch
        {
            static bool Prefix(GearListViewController __instance, ref ICharacterCustomizationItem item)
            {
                if (item is ICustomGearInfo)
                {
                    var selectedItem = item as ICustomGearInfo;
                    if (selectedItem == null) return true;

                    if (selectedItem is CustomFolderInfo)
                    {
                        var folder = selectedItem as CustomFolderInfo;
                        if (folder.GetName() == "..\\")
                        {
                            CustomGearManager.MoveUpDirectory();
                            // Somehow figure out the parent's index and set it to highlight
                        }
                        else
                        {
                            CustomGearManager.SetCurrentFolder(folder);
                            // Set it to the first (real) item in the list, not the ../
                            __instance.HighlightIndex(1);
                        }

                        return false;
                    }
                    else
                    {
                        if (selectedItem.GearInfo is BoardGearInfo) 
                            item = selectedItem.GearInfo as BoardGearInfo;
                        else if (selectedItem.GearInfo is CharacterGearInfo) 
                            item = selectedItem.GearInfo as CharacterGearInfo;
                    }
                }

                return true;
            }
        }
    }
}
