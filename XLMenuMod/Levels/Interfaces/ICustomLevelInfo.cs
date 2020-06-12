namespace XLMenuMod.Levels.Interfaces
{
    public interface ICustomLevelInfo
    {
        ICustomLevelInfo Parent { get; set; }
        bool IsFavorite { get; set; }

        string GetName();
    }
}
