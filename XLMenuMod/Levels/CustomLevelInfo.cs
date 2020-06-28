using Newtonsoft.Json;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo
    {
        [JsonIgnore]
        public CustomInfo Info { get; set; }

        // This is here for JSON serialization purposes
        public CustomLevelInfo()
        {
            Info = new CustomInfo { ParentObject = this };
        }

        public CustomLevelInfo(LevelInfo level)
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            isAssetBundle = level.isAssetBundle;

            Info = new CustomInfo(level.name, level.path, null)
            {
                ParentObject = this
            };
        }

        public CustomLevelInfo(LevelInfo level, CustomFolderInfo parent) : this(level)
        {
            Info.Parent = parent;
        }
    }
}