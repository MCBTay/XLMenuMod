using Newtonsoft.Json;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo, ICustomLevelInfo
    {
        [JsonIgnore]
        public ICustomLevelInfo Parent { get; set; }
        

        [JsonIgnore]
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }

        public CustomLevelInfo(LevelInfo level) 
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            isAssetBundle = level.isAssetBundle;

            Parent = null;
            IsFavorite = false;
        }

        public CustomLevelInfo(LevelInfo level, CustomFolderInfo parent) : this(level)
        {
            Parent = parent;
        }
    }
}
