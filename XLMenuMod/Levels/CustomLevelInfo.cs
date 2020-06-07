using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo, ICustomLevelInfo
    {
        public ICustomLevelInfo Parent { get; set; }
        public LevelInfo LevelInfo { get; set; }
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }
        public LevelInfo GetLevelInfo() { return LevelInfo; }

        public CustomLevelInfo(LevelInfo level) 
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            isAssetBundle = level.isAssetBundle;
            LevelInfo = level;

            Parent = null;
            IsFavorite = false;
        }

        public CustomLevelInfo(LevelInfo level, CustomFolderInfo parent) : this(level)
        {
            Parent = parent;
        }
    }
}
