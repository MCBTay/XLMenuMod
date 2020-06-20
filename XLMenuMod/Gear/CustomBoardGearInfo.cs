using System;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomBoardGearInfo : BoardGearInfo, ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime ModifiedDate { get; set; }

        public CustomBoardGearInfo(GearInfoSingleMaterial source) : base(source) { }

        public CustomBoardGearInfo(string name, string type, bool isCustom, TextureChange[] textureChanges, string[] tags) : base(name, type, isCustom, textureChanges, tags)
        {
            Parent = null;
            IsFavorite = false;
        }

        public DateTime GetModifiedDate() { return ModifiedDate; }
        public DateTime GetModifiedDate(bool ascending) { return ModifiedDate; }
    }
}
