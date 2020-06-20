using HarmonyLib;
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

                Traverse.Create(__instance).Method("UpdateList").GetValue();
            }
        }
    }
}
