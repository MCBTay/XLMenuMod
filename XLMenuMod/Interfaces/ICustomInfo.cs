using System;

namespace XLMenuMod.Interfaces
{
    public interface ICustomInfo
    {
        string Name { get; set; }
        string Path { get; set; }

        long Size { get; set; }

        bool IsFavorite { get; set; } // Not yet implemented
        
        bool IsFolder { get; set; }
        object ParentObject { get; set; }

        DateTime ModifiedDate { get; set; }
        DateTime LastUsage { get; set; }
        ICustomInfo Parent { get; set; }
        int UsageCount { get; set; }

        string GetName();
        string GetPath();
        DateTime GetModifiedDate();
        DateTime GetModifiedDate(bool ascending);
        int GetUsageCount();
        DateTime GetLastUsage();
        object GetParentObject();


    }
}
