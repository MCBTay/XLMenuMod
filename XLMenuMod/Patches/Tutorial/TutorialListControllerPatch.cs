using HarmonyLib;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Tutorial
{
    [HarmonyPatch(typeof(TutorialListController), nameof(TutorialListController.ConfigureListItemView))]
    public static class ConfigureListItemViewPatch
    {
        public static void Postfix(ref MVCListItemView itemView)
        {
            itemView.colors = Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
        }
    }
}
