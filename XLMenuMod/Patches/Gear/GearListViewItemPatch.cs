using HarmonyLib;

namespace XLMenuMod.Patches.Gear
{
    public class GearListViewItemPatch
    {
        [HarmonyPatch(typeof(GearListViewItem), nameof(GearListViewItem.Item), MethodType.Setter)]
        static class ItemPatch
        {
            static void Postfix(GearListViewItem __instance, LevelInfo value)
            {
                if (__instance.Label.text.StartsWith("\\"))
                {
                    if (Main.BlueSprites != null)
                    {
                        __instance.Label.spriteAsset = Main.BlueSprites;
                        __instance.Label.SetText(__instance.Label.text.Replace("\\", "<sprite=10 tint=1> "));
                    }

                }
                else if (__instance.Label.text.Equals("..\\"))
                {
                    if (Main.BlueSprites != null)
                    {
                        __instance.Label.spriteAsset = Main.BlueSprites;
                        __instance.Label.SetText(__instance.Label.text.Replace("..\\", "<sprite=9 tint=1> Go Back"));
                    }
                }
            }
        }
    }
}
