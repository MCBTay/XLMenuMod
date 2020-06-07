using System.Collections.Generic;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomCharacterGearInfo : CharacterGearInfo, ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public ICharacterCustomizationItem GearInfo { get; set; }
        public List<ICustomGearInfo> Children { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsFavorite { get; set; }

        public CustomCharacterGearInfo(GearInfoSingleMaterial source) : base(source) { }

        public CustomCharacterGearInfo(string name, string type, bool isCustom, TextureChange[] textureChanges, string[] tags) : base(name, type, isCustom, textureChanges, tags)
        {
            Parent = null;
            GearInfo = null;
            Children = new List<ICustomGearInfo>();
            IsDirectory = false;
            IsFavorite = false;
        }
    }
}
