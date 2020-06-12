using Harmony12;
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

                CustomGearManager.LoadNestedGear(__instance.visibleGear);

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
