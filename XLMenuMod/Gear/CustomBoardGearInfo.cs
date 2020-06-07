using System.Collections.Generic;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomBoardGearInfo : BoardGearInfo, ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public ICharacterCustomizationItem GearInfo { get; set; }
        public bool IsFavorite { get; set; }

        public CustomBoardGearInfo(GearInfoSingleMaterial source) : base(source) { }

        public CustomBoardGearInfo(string name, string type, bool isCustom, TextureChange[] textureChanges, string[] tags) : base(name, type, isCustom, textureChanges, tags)
        {
            Parent = null;
            GearInfo = null;
            IsFavorite = false;
        }
    }
}
