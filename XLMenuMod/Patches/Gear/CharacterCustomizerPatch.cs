﻿using HarmonyLib;
using System.Collections.Generic;
using SkaterXL.Data;
using XLMenuMod.Utilities;
using XLMenuMod.Utilities.Gear;
using XLMenuMod.Utilities.Gear.Interfaces;

namespace XLMenuMod.Patches.Gear
{
	public static class CharacterCustomizerPatch
    {
        [HarmonyPatch(typeof(CharacterCustomizer), nameof(CharacterCustomizer.HasEquipped), new[] { typeof(ICharacterCustomizationItem) })]
        static class HasEquippedPatch
        {
            static bool Prefix(ref ICharacterCustomizationItem item)
            {
                if (item is ICustomGearInfo customGear)
                {
                    if (customGear.Info is CustomFolderInfo) return false;

                    if (customGear is CustomBoardGearInfo boardGear) item = boardGear;
                    if (customGear is CustomCharacterGearInfo charGear) item = charGear;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterCustomizer), nameof(CharacterCustomizer.PreviewItem))]
        static class PreviewItemPatch
        {
	        static bool Prefix(GearInfo preview)
	        {
		        if (preview is CustomGearFolderInfo highlightedFolder) return false;
		        return true;
	        }
        }

        [HarmonyPatch(typeof(CharacterCustomizer), nameof(CharacterCustomizer.LoadGearAsync))]
        static class LoadGearAsyncPatch
        {
            static bool Prefix(ref GearInfo gear)
            {
                if (gear is ICustomGearInfo customGear)
                {
                    if (customGear.Info is CustomFolderInfo) return false;

                    if (customGear is CustomBoardGearInfo boardGear) gear = boardGear;
                    if (customGear is CustomCharacterGearInfo charGear) gear = charGear;
                }

                return true;
            }
        }
    }
}
