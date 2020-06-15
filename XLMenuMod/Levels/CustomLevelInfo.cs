using Newtonsoft.Json;
using System;
using System.IO;
using UnityModManagerNet;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo, ICustomLevelInfo
    {
        [JsonIgnore]
        public ICustomLevelInfo Parent { get; set; }

        [JsonIgnore]
        public int PlayCount { get; set; }

        [JsonIgnore]
        public long Size { get; set; }

        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }


        [JsonIgnore]
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }

        public CustomLevelInfo(LevelInfo level) 
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            isAssetBundle = level.isAssetBundle;

            Size = new FileInfo(path).Length;

            Parent = null;
            IsFavorite = false;
        }

        public CustomLevelInfo(LevelInfo level, CustomFolderInfo parent) : this(level)
        {
            Parent = parent;
        }
    }
}
