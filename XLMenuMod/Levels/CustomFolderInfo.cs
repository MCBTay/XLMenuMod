using Newtonsoft.Json;
using System.Collections.Generic;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomFolderInfo : LevelInfo, ICustomLevelInfo
    {
        [JsonIgnore]
        public ICustomLevelInfo Parent { get; set; }

        [JsonIgnore]
        public List<ICustomLevelInfo> Children { get; set; }

        [JsonIgnore]
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }


        public CustomFolderInfo()
        {
            Parent = null;
            Children = new List<ICustomLevelInfo>();
        }
    }
}
