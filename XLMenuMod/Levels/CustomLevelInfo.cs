using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo, ICustomLevelInfo
    {
        [JsonIgnore]
        public ICustomLevelInfo Parent { get; set; }

        public int PlayCount { get; set; }

        public DateTime LastPlayTime { get; set; }

        [JsonIgnore]
        public long Size { get; set; }

        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }


        [JsonIgnore]
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }
        public string GetHash() { return hash; }

        public DateTime GetModifiedDate() { return ModifiedDate; }
        public DateTime GetModifiedDate(bool ascending) { return ModifiedDate; }

        public int GetPlayCount(List<ICustomLevelInfo> source = null) { return PlayCount; }

        public DateTime GetLastPlayTime() { return LastPlayTime; }

        public CustomLevelInfo()
        {
            Parent = null;
            IsFavorite = false;
        }

        public CustomLevelInfo(LevelInfo level) 
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            isAssetBundle = level.isAssetBundle;

            var fileInfo = new FileInfo(path);
            Size = fileInfo.Length;
            ModifiedDate = fileInfo.LastWriteTime;

            LastPlayTime = DateTime.MinValue;
        }

        public CustomLevelInfo(LevelInfo level, CustomFolderInfo parent) : this(level)
        {
            Parent = parent;
        }
    }
}
