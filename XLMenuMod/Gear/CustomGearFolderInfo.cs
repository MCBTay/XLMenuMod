using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using XLMenuMod.Gear.Interfaces;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear
{
	public class CustomGearFolderInfo : GearInfo, ICustomGearInfo, ICustomFolderInfo
    {
        [JsonIgnore]
        public ICustomInfo Info { get; set; }

        public CustomFolderInfo FolderInfo
        {
            get { return Info as CustomFolderInfo; }
            set { Info = value; }
        }

        public TMP_SpriteAsset CustomSprite { get; set; }

        public string GetName() { return Info?.GetName(); }
        

        public CustomGearFolderInfo(string name, string path, CustomFolderInfo parent) : base(name, "Folder", true, new string[]{})
        {
            Info = new CustomFolderInfo(name, path, parent) { ParentObject = this };

            if (name != "..\\")
            {
                var backFolder = FolderInfo.Children.First();
                backFolder.ParentObject = new CustomGearFolderInfo("..\\", backFolder.GetPath(), parent);
            }
        }

        public override bool EqualPaths(GearInfo other) { throw new System.NotImplementedException(); }
        public override IEnumerable<MaterialChange> GetMaterialChanges() { throw new System.NotImplementedException(); }
    }
}
