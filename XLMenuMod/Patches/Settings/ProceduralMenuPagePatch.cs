using HarmonyLib;
using System.Linq;
using UnityEngine.UI;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Settings
{
    public static class ProceduralMenuPagePatch
    {
        [HarmonyPatch(typeof(ProceduralMenuPage), nameof(ProceduralMenuPage.UpdatePage))]
        public static class UpdatePagePatch
        {
            static void Postfix(ProceduralMenuPage __instance)
            {
                if (__instance.items == null || !__instance.items.Any()) return;

                foreach (var item in __instance.items)
                {
                    switch (item)
                    {
                        case FloatSliderItem floatSlider:
                            floatSlider.selectable.colors = GetColors();
                            break;
                        case IntSliderItem intSlider:
                            intSlider.selectable.colors = GetColors(true);
                            break;
                        case ColorSliderItem colorSlider:
                            colorSlider.selectable.colors = GetColors(true);
                            break;
                        case EnumSliderItem enumSlider:
                            enumSlider.selectable.colors = GetColors();
                            break;
                        case StringListSliderItem stringListSlider:
                            stringListSlider.selectable.colors = GetColors();
                            break;
                        case StringListIntSliderItem stringListIntSlider:
                            stringListIntSlider.selectable.colors = GetColors();
                            break;
                        case InputFieldItem inputField:
                            inputField.selectable.label.color = GetColors().normalColor;
                            break;
                        case ButtonItem button:
                            button.selectable.colors = GetColors();
                            break;
                        case ToggleItem toggle:
                            toggle.selectable.colors = GetColors();
                            break;

                    }
                }
            }

            private static ColorBlock GetColors(bool isSlider = false)
            {
                var darkMode = isSlider ? UserInterfaceHelper.DarkModeSliderText : UserInterfaceHelper.DarkModeText;
                var lightMode = isSlider ? UserInterfaceHelper.DefaultSliderText : UserInterfaceHelper.DefaultText;

                return Main.Settings.EnableDarkMode ? darkMode : lightMode;
            }
        }
    }
}
