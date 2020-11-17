using System.Collections.Generic;
using System.Linq;
using XLMenuMod.Utilities.Gear.Interfaces;
using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Utilities.Gear
{
	public class CustomCharacterBodyInfo : CharacterBodyInfo, ICustomGearInfo
	{
		public ICustomInfo Info { get; set; }

		public CustomCharacterBodyInfo(string name, string type, bool isCustom, List<MaterialChange> materialChanges, string[] tags) : base(name, type, isCustom, materialChanges, tags)
		{
			// For now all I saw was one texture change per gear type, so assuming first.
			var textureChange = materialChanges?.FirstOrDefault()?.textureChanges?.FirstOrDefault();
			if (textureChange != null)
			{
				Info = new CustomInfo(name, textureChange.texturePath, null, isCustom) { ParentObject = this };
			}
		}
    }
}
