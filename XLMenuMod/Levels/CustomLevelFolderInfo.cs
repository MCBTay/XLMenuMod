using Newtonsoft.Json;
using System.Linq;
using TMPro;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Levels
{
	public class CustomLevelFolderInfo : LevelInfo, ICustomFolderInfo
    {
        [JsonIgnore]
        public CustomFolderInfo FolderInfo { get; set; }

        public TMP_SpriteAsset CustomSprite { get; set; }

        public CustomLevelFolderInfo(string name, string path, CustomFolderInfo parent) : base(path, false)
        {
            this.name = name;
            this.path = path;

            FolderInfo = new CustomFolderInfo(name, path, parent) { ParentObject = this };

            if (name != "..\\")
            {
                var backFolder = FolderInfo.Children.First();
                backFolder.ParentObject = new CustomLevelFolderInfo("..\\", backFolder.GetPath(), parent);
            }
        }
    }
}
