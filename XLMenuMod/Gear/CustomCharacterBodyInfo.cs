using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLMenuMod.Gear.Interfaces;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear
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
