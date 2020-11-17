using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Utilities.Gear.Interfaces
{
    public interface ICustomGearInfo : ICharacterCustomizationItem
    {
        ICustomInfo Info { get; set; }
    }
}
