using Harmony12;
using XLMenuMod.Gear;

namespace XLMenuMod.Patches.Gear
{
    public class GearTypeFilteringControllerPatch
    {
        [HarmonyPatch(typeof(GearTypeFilteringController), "SetFilter")]
        public static class SetFilterPatch
        {
            public static void Prefix()
            {
                CustomGearManager.SetCurrentFolder(null);
            }

            public static void Postfix(int ___currentFilterIndex)
            {
                CustomGearManager.CurrentGearFilterIndex = ___currentFilterIndex;
            }
        }
    }
}
