using System;
using System.Collections.Generic;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomFolderInfo : ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public List<ICustomGearInfo> Children { get; set; }
        public bool IsFavorite { get; set; }
        public string Name { get; set; }
        public ICharacterCustomizationItem GearInfo { get; set; }

        public CustomFolderInfo() 
        {
            Parent = null;
            Children = new List<ICustomGearInfo>();
            IsFavorite = false;
            Name = string.Empty;
            GearInfo = null;
        }

        public string GetName() { return Name; }
    }
}
