using System;

namespace XLMenuMod.Gear.Interfaces
{
    public interface ICustomGearInfo : ICharacterCustomizationItem
    {
        ICustomGearInfo Parent { get; set; }
        bool IsFavorite { get; set; }
        DateTime ModifiedDate { get; set; }

        DateTime GetModifiedDate();
        DateTime GetModifiedDate(bool ascending);
    }
}
