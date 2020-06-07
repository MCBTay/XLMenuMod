namespace XLMenuMod.Gear.Interfaces
{
    public interface ICustomGearInfo : ICharacterCustomizationItem
    {
        ICustomGearInfo Parent { get; set; }
        ICharacterCustomizationItem GearInfo { get; set; }
        bool IsFavorite { get; set; }
    }
}
