using System.Linq;
using Newtonsoft.Json;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelFolderInfo : LevelInfo
    {
        [JsonIgnore]
        public CustomFolderInfo FolderInfo { get; set; }

        public CustomLevelFolderInfo(string name, string path, ICustomInfo parent)
        {
            this.name = name;
            this.path = path;

            FolderInfo = new CustomFolderInfo(name, path, parent)
            {
                ParentObject = this,
            };

            if (name != "..\\")
            {
                var backFolder = FolderInfo.Children.First();
                backFolder.ParentObject = new CustomLevelFolderInfo("..\\", backFolder.GetPath(), parent);
            }
        }
    }
}
