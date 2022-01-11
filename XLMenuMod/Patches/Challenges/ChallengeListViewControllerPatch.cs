using HarmonyLib;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Challenges
{
    public static class ChallengeListViewControllerPatch
    {
        [HarmonyPatch(typeof(ChallengeListViewController), nameof(ChallengeListViewController.ConfigureListItemView))]
        public static class UpdateListPatch
        {
            public static void Postfix(ref MVCListItemView itemView)
            {
                itemView.colors = Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
            }
        }
    }
}
