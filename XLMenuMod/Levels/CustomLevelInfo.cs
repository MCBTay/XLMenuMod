using Newtonsoft.Json;

namespace XLMenuMod.Levels
{
    public class CustomLevelInfo : LevelInfo
    {
        [JsonIgnore]
        public CustomInfo Info { get; set; }

		// This is here for JSON serialization purposes
		public CustomLevelInfo() : base(string.Empty, false)
		{
			Info = new CustomInfo { ParentObject = this };
		}

		public CustomLevelInfo(LevelInfo level) : base(level.path, level.isAssetBundle)
        {
            name = level.name;
            hash = level.hash;
            path = level.path;
            previewImage = level.previewImage;
            isAssetBundle = level.isAssetBundle;

            Info = new CustomInfo(level.name, level.path, null, isAssetBundle)
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