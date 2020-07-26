using HarmonyLib;
using UnityModManagerNet;
using XLMenuMod.Gear;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches
{
	static class MVCListViewPatch
	{
		[HarmonyPatch(typeof(MVCListView), "Header_OnPreviousCategory")]
		public static class Header_OnPreviewCategoryPatch
		{
			static void Prefix(MVCListView __instance)
			{
				if (__instance.DataSource != null && __instance.currentIndexPath.depth >= 3)
				{
					while (__instance.currentIndexPath.depth > 3)
					{
						Traverse.Create(__instance).Property("currentIndexPath").SetValue(__instance.currentIndexPath.Up());
					}

					if (__instance.DataSource is LevelSelectionController) CustomLevelManager.Instance.CurrentFolder = null;
					else if (__instance.DataSource is GearSelectionController) CustomGearManager.Instance.CurrentFolder = null;
				}
			}
		}

		[HarmonyPatch(typeof(MVCListView), "Header_OnNextCategory")]
		public static class Header_OnNextCategoryPatch
		{
			static void Prefix(MVCListView __instance)
			{
				if (__instance.DataSource != null && __instance.currentIndexPath.depth >= 3)
				{
					while (__instance.currentIndexPath.depth > 3)
					{
						Traverse.Create(__instance).Property("currentIndexPath").SetValue(__instance.currentIndexPath.Up());
					}

					if (__instance.DataSource is LevelSelectionController) CustomLevelManager.Instance.CurrentFolder = null;
					else if (__instance.DataSource is GearSelectionController) CustomGearManager.Instance.CurrentFolder = null;
				}
			}
		}
	}
}
