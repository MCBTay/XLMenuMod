using System;
using UnityModManagerNet;

namespace XLMenuMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public bool DisableBToMoveUpDirectory { get; set; }

        public Settings() : base()
        {
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}
