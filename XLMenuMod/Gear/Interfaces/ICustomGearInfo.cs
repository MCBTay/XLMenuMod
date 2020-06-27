using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear.Interfaces
{
    public interface ICustomGearInfo : ICharacterCustomizationItem
    {
        ICustomInfo Info { get; set; }
    }
}
