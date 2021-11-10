using HarmonyLib;
using XLMenuMod.Utilities.Gear;

namespace XLMenuMod.Patches
{
	static class MVCListViewPatch
	{
		[HarmonyPatch(typeof(MVCListView), "Header_OnPreviousCategory")]
		public static class Header_OnPreviewCategoryPatch
		{
			static void Prefix(MVCListView __instance)
			{
				__instance.WalkUpFolders();
			}
		}

		[HarmonyPatch(typeof(MVCListView), "Header_OnNextCategory")]
		public static class Header_OnNextCategoryPatch
		{
			static void Prefix(MVCListView __instance)
			{
				__instance.WalkUpFolders();
			}
		}
	}
}
