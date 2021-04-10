using HarmonyLib;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Multiplayer
{
	public class MultiplayerRoomInfoPatch
	{
		[HarmonyPatch(typeof(MultiplayerRoomInfo), nameof(MultiplayerRoomInfo.ConfigureListItemView))]
		public static class ConfigureListItemViewPatch
		{
			static void Postfix(MultiplayerRoomInfo __instance, IndexPath index, ref MVCListItemView itemView)
			{
				itemView.colors = Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
			}
		}
	}
}
