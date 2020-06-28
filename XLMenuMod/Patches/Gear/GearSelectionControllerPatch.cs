using HarmonyLib;
using System.Linq;
using XLMenuMod.Gear;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Patches.Gear
{
    public class GearSelectionControllerPatch
    {
        [HarmonyPatch(typeof(GearSelectionController), nameof(GearSelectionController.ShowGearWithFilter))]
        public static class ShowGearWithFilterPatch
        {
            public static void Postfix(GearSelectionController __instance, VisibleGearOrigin ___visibleGearOrigin)
            {
                if (___visibleGearOrigin != VisibleGearOrigin.Custom)
                {
                    CustomGearManager.Instance.CurrentFolder = null;
                    return;
                }

                CustomGearManager.Instance.LoadNestedItems();

                __instance.visibleGear.Clear();
                __instance.visibleGear.AddRange(CustomGearManager.Instance.NestedItems.Select(x => x.GetParentObject() as ICustomGearInfo));

                Traverse.Create(__instance).Method("UpdateList").GetValue();
            }
        }
    }

    [HarmonyPatch(typeof(GearSelectionController), "Awake")]
    public static class AwakePatch
    {
        static void Postfix(GearSelectionController __instance)
        {
            CustomGearManager.Instance.SortLabel = UserInterfaceHelper.CreateSortLabel(__instance.gearTypeFiltering.gearCategoryButton.label, __instance.gearTypeFiltering.gearCategoryButton.transform, ((GearSortMethod)CustomGearManager.Instance.CurrentSort).ToString());
        }
    }
}
