using System;
using UnityEngine;
using UnityModManagerNet;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public bool DisableBToMoveUpDirectory { get; set; }
        public FontSizePreset FontSize { get; set; } = FontSizePreset.Normal;
        public bool EnableDarkMode { get; set; }
        public bool HideOfficialGear { get; set; }

        public Settings() : base()
        {
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);

            UserInterfaceHelper.Instance.ToggleDarkMode(EnableDarkMode);
        }

        public void OnSettingsGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            DisableBToMoveUpDirectory = GUILayout.Toggle(DisableBToMoveUpDirectory, new GUIContent("Disable B/O Button to Move Up Directory"));
            EnableDarkMode = GUILayout.Toggle(EnableDarkMode, new GUIContent("Enable Dark Mode"));
            HideOfficialGear = GUILayout.Toggle(HideOfficialGear, new GUIContent("Hide Official Gear"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Gear/Map Font Size: ");
            string[] fontOptions = { FontSizePreset.Normal.ToString(), FontSizePreset.Small.ToString(), FontSizePreset.Smaller.ToString() };
            FontSize = (FontSizePreset)GUILayout.SelectionGrid((int)FontSize, fontOptions, 3);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
