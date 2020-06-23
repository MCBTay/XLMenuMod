using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomCharacterGearInfo : CharacterGearInfo, ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public List<ICustomGearInfo> Children { get; set; }
        public bool IsFavorite { get; set; }
        public DateTime ModifiedDate { get; set; }

        public CustomCharacterGearInfo(GearInfoSingleMaterial source) : base(source) { }

        public CustomCharacterGearInfo(string name, string type, bool isCustom, TextureChange[] textureChanges, string[] tags) : base(name, type, isCustom, textureChanges, tags)
        {
            Parent = null;
            Children = new List<ICustomGearInfo>();
            IsFavorite = false;

            // For now all I saw was one texture change per gear type, so assuming first.
            var textureChange = textureChanges?.FirstOrDefault();
            if (textureChange != null)
            {
                var fileInfo = new FileInfo(textureChange.texturePath);
                ModifiedDate = fileInfo.LastWriteTime;
            }
        }

        public DateTime GetModifiedDate() { return ModifiedDate; }
        public DateTime GetModifiedDate(bool ascending) { return ModifiedDate; }
    }
}
