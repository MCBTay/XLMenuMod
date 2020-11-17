using TMPro;

namespace XLMenuMod.Utilities.Interfaces
{
    public interface ICustomFolderInfo
    {
        CustomFolderInfo FolderInfo { get; set; }
        TMP_SpriteAsset CustomSprite { get; set; }
    }
}
