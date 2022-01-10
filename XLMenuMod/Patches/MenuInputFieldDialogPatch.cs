using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches
{
    public static class MenuInputFieldDialogPatch
    {
        [HarmonyPatch(typeof(MenuInputFieldDialog), "Awake")]
        public static class AwakePatch
        {
            static void Postfix(MenuInputFieldDialog __instance)
            {
                var images = __instance.gameObject.GetComponentsInChildren<Image>(true).Where(x => x.name.StartsWith("Background"));

                foreach (var image in images)
                {
                    image.sprite = Main.Settings.EnableDarkMode && SpriteHelper.DarkModeBackground != null ? SpriteHelper.DarkModeBackground : SpriteHelper.OriginalBackground;
                }

                var label = __instance.gameObject.GetComponentInChildren<TMP_Text>(true);
                label.ToggleDarkMode(Main.Settings.EnableDarkMode);

                var input = __instance.gameObject.GetComponentInChildren<TMP_InputField>(true);
                if (input == null) return;

                input.image.color = Main.Settings.EnableDarkMode ? Color.gray : Color.white;
            }
        }
    }
}
