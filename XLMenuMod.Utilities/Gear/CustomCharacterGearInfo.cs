using System.Linq;
using XLMenuMod.Utilities.Gear.Interfaces;
using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Utilities.Gear
{
	public class CustomCharacterGearInfo : CharacterGearInfo, ICustomGearInfo
    {
        public ICustomInfo Info { get; set; }

        public CustomCharacterGearInfo(string name, string type, bool isCustom, TextureChange[] textureChanges, string[] tags) : base(name, type, isCustom, textureChanges, tags)
        {
            // For now all I saw was one texture change per gear type, so assuming first.
            var textureChange = textureChanges?.FirstOrDefault();
            if (textureChange != null)
            {
                Info = new CustomInfo(name, textureChange.texturePath, null, isCustom) { ParentObject = this };
            }
        }
    }
}
