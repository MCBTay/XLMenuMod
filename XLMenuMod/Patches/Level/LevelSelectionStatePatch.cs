using GameManagement;
using HarmonyLib;
using Rewired;
using UnityEngine.EventSystems;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    static class LevelSelectionStatePatch
    {
        [HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnUpdate))]
        static class OnUpdatePatch
        {
            static bool Prefix(LevelSelectionState __instance)
            {
	            Player player = PlayerController.Instance.inputController.player;
	            var levelSelection = UnityEngine.Object.FindObjectOfType<LevelSelectionController>();

                if (player.GetButtonDown("Y"))
                {
                    UISounds.Instance?.PlayOneShotSelectionChange();

                    CustomLevelManager.Instance.OnNextSort<LevelSortMethod>();
                    return false;
                }

                if (CustomLevelManager.Instance.CurrentFolder == null) return true;
                if (!player.GetButtonDown("B")) return true;

                if (!Main.Settings.DisableBToMoveUpDirectory)
                {
                    UISounds.Instance?.PlayOneShotSelectMajor();

                    CustomLevelManager.Instance.CurrentFolder = CustomLevelManager.Instance.CurrentFolder.Parent;

                    EventSystem.current.SetSelectedGameObject(null);
                    levelSelection.listView.UpdateList();

                    return false;
                }

                CustomLevelManager.Instance.CurrentFolder = null;
                return true;
            }
        }

        [HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnEnter))]
        static class OnEnterPatch
        {
            static void Postfix()
            {
                CustomLevelManager.Instance.LoadNestedItems();
            }
        }

        [HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnExit))]
        static class OnExitPatch
        {
            static void Postfix()
            {
                CustomLevelManager.Instance.CurrentFolder = null;
            }
        }
    }
}
