using System;
using System.Collections.Generic;

namespace XLMenuMod.Levels.Interfaces
{
    public interface ICustomLevelInfo
    {
        ICustomLevelInfo Parent { get; set; }
        bool IsFavorite { get; set; }
        long Size { get; set; }
        int PlayCount { get; set; }
        DateTime LastPlayTime { get; set; }
        DateTime ModifiedDate { get; set; }

        string GetName();
        string GetHash();
        DateTime GetModifiedDate();
        DateTime GetModifiedDate(bool ascending);
        int GetPlayCount(List<ICustomLevelInfo> source = null);
        DateTime GetLastPlayTime();
    }
}