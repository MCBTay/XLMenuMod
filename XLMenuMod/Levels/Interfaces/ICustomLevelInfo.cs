using System;

namespace XLMenuMod.Levels.Interfaces
{
    public interface ICustomLevelInfo
    {
        ICustomLevelInfo Parent { get; set; }
        bool IsFavorite { get; set; }
        long Size { get; set; }
        int PlayCount { get; set; }
        DateTime ModifiedDate { get; set; }

        string GetName();
        DateTime GetModifiedDate();
        DateTime GetModifiedDate(bool ascending);
    }
}
