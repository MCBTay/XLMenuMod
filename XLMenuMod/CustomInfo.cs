using System;
using System.IO;
using XLMenuMod.Interfaces;

namespace XLMenuMod
{
    public class CustomInfo : ICustomInfo
    {
        public CustomFolderInfo Parent { get; set; }
        public long Size { get; set; }
        public DateTime ModifiedDate { get; set; }

        public bool IsFavorite { get; set; }
        public bool IsFolder { get; set; }
        public DateTime LastUsage { get; set; }
        public int UsageCount { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public object ParentObject { get; set; }

        public string GetName() { return Name; }
        public string GetPath() { return Path; }

        public virtual DateTime GetModifiedDate() { return ModifiedDate; }
        public virtual DateTime GetModifiedDate(bool ascending) { return ModifiedDate; }



        public int GetUsageCount() { return UsageCount; }

        public DateTime GetLastUsage() { return LastUsage; }

        public object GetParentObject() { return ParentObject; }

        public CustomInfo()
        {
            Parent = null;
            IsFavorite = false;
            IsFolder = false;
        }

        public CustomInfo(string name, string path, bool getFileSize = true) : this()
        {
            Name = name;
            Path = path;

            if (getFileSize)
            {
                var fileInfo = new FileInfo(path);
                Size = fileInfo.Length;
                ModifiedDate = fileInfo.LastWriteTime;
            }

            LastUsage = DateTime.MinValue;
        }

        public CustomInfo(string name, string path, CustomFolderInfo parent, bool getFileSize = true) : this(name, path, getFileSize)
        {
            Parent = parent;
        }
    }
}
