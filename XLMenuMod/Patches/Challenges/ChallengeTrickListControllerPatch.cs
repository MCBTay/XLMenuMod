using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Challenges
{
	public static class ChallengeTrickListControllerPatch
	{
		[HarmonyPatch(typeof(ChallengeTrickListController), nameof(ChallengeTrickListController.UpdateList))]
		public static class UpdateListPatch
		{
			public static void Postfix(ChallengeTrickListController __instance)
			{
				var items = Traverse.Create(__instance).Field("ItemViews").GetValue<List<ChallengeTrickItemView>>();
				if (items != null && items.Any())
				{
					foreach (var item in items)
					{
						UserInterfaceHelper.Instance.UpdateLabelColor(item.label, Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText);
						item.stateImage.color = Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText.normalColor : UserInterfaceHelper.DefaultText.normalColor;
					}
				}
			}
		}
	}
}
