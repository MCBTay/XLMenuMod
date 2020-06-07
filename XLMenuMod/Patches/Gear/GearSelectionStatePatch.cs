using GameManagement;
using Harmony12;
using System.Linq;
using XLMenuMod.Gear;

namespace XLMenuMod.Patches.Gear
{
    public class GearSelectionStatePatch
    {
        [HarmonyPatch(typeof(GearSelectionState), nameof(GearSelectionState.OnUpdate))]
        static class OnUpdatePatch
        {
            static bool Prefix()
            {
                if (CustomGearManager.CurrentFolder == null) return true;
                if (!PlayerController.Instance.inputController.player.GetButtonDown("B")) return true;

                if (!Main.Settings.DisableBToMoveUpDirectory)
                {
                    UISounds.Instance?.PlayOneShotSelectMajor();
                    CustomGearManager.MoveUpDirectory();
                    return false;
                }
                    
                CustomGearManager.CurrentFolder = null;
                return true;
            }
        }
    }
}
