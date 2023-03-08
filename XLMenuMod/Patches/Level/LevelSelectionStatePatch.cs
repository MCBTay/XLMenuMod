using GameManagement;
using HarmonyLib;
using Rewired;
using XLMenuMod.Utilities.Levels;

namespace XLMenuMod.Patches.Level
{
	static class LevelSelectionStatePatch
    {
        [HarmonyPatch(typeof(LevelSelectionState), nameof(LevelSelectionState.OnUpdate))]
        static class OnUpdatePatch
        {
            static bool Prefix()
            {
	            Player player = PlayerController.Main.input;
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

                    var currentIndexPath = Traverse.Create(levelSelection.listView).Property<IndexPath>("currentIndexPath");
                    currentIndexPath.Value = levelSelection.listView.currentIndexPath.Up();

                    levelSelection.listView.UpdateList();
                    levelSelection.listView.SetHighlighted(currentIndexPath.Value, true);

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
