using GameManagement;
using Harmony12;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    static class LevelSelectionStatePatch
    {
        //[HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnEnter))]
        //static class OnEnterPatch
        //{
        //    static void Postfix()
        //    {
        //        CustomLevelManager.LoadNestedLevels();
        //    }
        //}

        [HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnUpdate))]
        static class OnUpdatePatch
        {
            static bool Prefix()
            {
                if (CustomLevelManager.CurrentFolder == null) return true;
                if (!PlayerController.Instance.inputController.player.GetButtonDown("B")) return true;

                if (!Main.Settings.DisableBToMoveUpDirectory)
                {
                    UISounds.Instance?.PlayOneShotSelectMajor();
                    CustomLevelManager.MoveUpDirectory();
                    return false;
                }

                CustomLevelManager.CurrentFolder = null;
                return true;
            }
        }
    }
}
