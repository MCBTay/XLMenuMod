using HarmonyLib;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Multiplayer
{
	public static class MultiplayerRoomInfoPatch
	{
		[HarmonyPatch(typeof(MultiplayerRoomInfo), nameof(MultiplayerRoomInfo.ConfigureListItemView))]
		public static class ConfigureListItemViewPatch
		{
			static void Postfix(ref MVCListItemView itemView)
			{
				itemView.colors = Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
			}
		}
	}
}
