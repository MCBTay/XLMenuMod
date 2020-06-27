using HarmonyLib;
using UnityEngine.EventSystems;
using XLMenuMod.Gear;

namespace XLMenuMod.Patches.Gear
{
    public class GearTypeFilteringControllerPatch
    {
        [HarmonyPatch(typeof(GearTypeFilteringController), "SetFilter")]
        public static class SetFilterPatch
        {
            public static void Postfix(int ___currentFilterIndex, bool ___showCustomGear)
            {
                EventSystem.current.SetSelectedGameObject(null);

                CustomGearManager.CurrentGearFilterIndex = ___currentFilterIndex;
                CustomGearManager.SetCurrentFolder(null, ___showCustomGear);

                if (CustomGearManager.SortLabel != null)
                {
                    CustomGearManager.SortLabel.gameObject.SetActive(___showCustomGear);
                }
            }
        }
    }
}
