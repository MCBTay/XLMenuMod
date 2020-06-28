using GameManagement;
using HarmonyLib;
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
                if (PlayerController.Instance.inputController.player.GetButtonDown("Y"))
                {
                    UISounds.Instance?.PlayOneShotSelectionChange();

                    CustomGearManager.Instance.OnNextSort<GearSortMethod>();
                    return false;
                }

                if (CustomGearManager.Instance.CurrentFolder == null) return true;
                if (!PlayerController.Instance.inputController.player.GetButtonDown("B")) return true;

                if (!Main.Settings.DisableBToMoveUpDirectory)
                {
                    UISounds.Instance?.PlayOneShotSelectMajor();
                    CustomGearManager.Instance.CurrentFolder = CustomGearManager.Instance.CurrentFolder.Parent;
                    return false;
                }
                    
                CustomGearManager.Instance.CurrentFolder = null;
                return true;
            }
        }
    }
}
