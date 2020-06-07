using Harmony12;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Gear;

namespace XLMenuMod.Patches.Gear
{
    public class GearSelectionControllerPatch : GearSelectionController
    {
        [HarmonyPatch(typeof(GearSelectionController), nameof(GearSelectionController.ShowGearWithFilter))]
        public static class ShowGearWithFilterPatch
        {
            public static void Postfix(GearSelectionController __instance, VisibleGearOrigin ___visibleGearOrigin)
            {
                if (___visibleGearOrigin != VisibleGearOrigin.Custom)
                {
                    CustomGearManager.CurrentFolder = null;
                    return;
                }
                
                CustomGearManager.NestedCustomGear.Clear();

                foreach (var gear in __instance.visibleGear)
                {
                    var singleMaterialGear = gear as GearInfoSingleMaterial;

                    // For now all I saw was one texture change per gear type, so assuming first.
                    var textureChange = singleMaterialGear?.textureChanges?.FirstOrDefault();
                    if (textureChange == null) continue;

                    if (string.IsNullOrEmpty(textureChange.texturePath) || !textureChange.texturePath.StartsWith(SaveManager.Instance.CustomGearDir)) continue;

                    var textureSubPath = textureChange.texturePath.Replace(SaveManager.Instance.CustomGearDir + '\\', string.Empty);

                    if (string.IsNullOrEmpty(textureSubPath)) continue;

                    var folders = textureSubPath.Split('\\').ToList();
                    if (folders == null || !folders.Any()) continue;

                    if (folders.Count == 1 || Path.GetExtension(folders.First()).ToLower() == ".png")
                    {
                        // This gear item is at the root.
                        CustomGearManager.AddGear(singleMaterialGear);    
                        continue;
                    }

                    CustomFolderInfo parent = null;
                    foreach (var folder in folders)
                    {
                        if (Path.GetExtension(folder).ToLower() == ".png")
                        {
                            CustomGearManager.AddGear(singleMaterialGear, ref parent);
                        }
                        else
                        {
                            CustomGearManager.AddFolder(folder, ref parent);
                        }
                    }
                }

                CustomGearManager.NestedCustomGear = CustomGearManager.NestedCustomGear.OrderBy(x => x.GetName()).ToList();

                if (CustomGearManager.CurrentFolder == null && CustomGearManager.NestedCustomGear.Any())
                {
                    CustomGearManager.OriginalCustomGear.Clear();
                    CustomGearManager.OriginalCustomGear.AddRange(CustomGearManager.NestedCustomGear);
                }

                __instance.visibleGear.Clear();
                __instance.visibleGear.AddRange(CustomGearManager.NestedCustomGear);

                #region GearSelectionController.UpdateList() -- Can't call it because it's private.
                int index = __instance.visibleGear.FindIndex(item => __instance.previewCustomizer.HasEquipped(item));
                if (index < 0)
                    index = __instance.listView.GetHighlightedIndex();
                if (index < 0)
                    index = 0;
                EventSystem.current.SetSelectedGameObject((GameObject)null);
                __instance.listView.SetItems(__instance.visibleGear);
                __instance.listView.HighlightIndex(index);
                #endregion
            }
        }
    }
}
