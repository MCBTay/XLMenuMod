using Harmony12;
using XLMenuMod.Gear;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Patches.Gear
{
    public class CharacterCustomizerPatch
    {
        [HarmonyPatch(typeof(CharacterCustomizer), nameof(CharacterCustomizer.HasEquipped), new[] { typeof(ICharacterCustomizationItem) })]
        static class HasEquippedPatch
        {
            static bool Prefix(ref ICharacterCustomizationItem item)
            {
                if (item is ICustomGearInfo)
                {
                    var customGear = item as ICustomGearInfo;
                    if (customGear == null) return true;
                    if (customGear is CustomFolderInfo) return false;

                    item = customGear.GearInfo;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterCustomizer), nameof(CharacterCustomizer.LoadGearAsync))]
        static class LoadGearAsyncPatch
        {
            static bool Prefix(ref GearInfo gear)
            {
                if (gear is ICustomGearInfo)
                {
                    var customGear = gear as ICustomGearInfo;
                    if (customGear == null) return true;
                    if (customGear is CustomFolderInfo) return false;
                    
                    gear = customGear.GearInfo as GearInfo;
                }

                return true;
            }
        }
    }
}
