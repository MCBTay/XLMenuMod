using System.Collections.Generic;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomFolderInfo : LevelInfo, ICustomLevelInfo
    {
        public ICustomLevelInfo Parent { get; set; }
        public LevelInfo LevelInfo { get; set; }
        public List<ICustomLevelInfo> Children { get; set; }
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }

        public LevelInfo GetLevelInfo() { return LevelInfo; }

        public CustomFolderInfo()
        {
            Parent = null;
            LevelInfo = null;
            Children = new List<ICustomLevelInfo>();
        }
    }
}
