using System;
using UnityEngine;
using UnityModManagerNet;

namespace XLMenuMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public bool DisableBToMoveUpDirectory { get; set; }
        public FontSizePreset FontSize { get; set; } = FontSizePreset.Normal;
        public bool EnableDarkMode { get; set; }

        public Settings() : base()
        {
        }

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnSettingsGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            DisableBToMoveUpDirectory = GUILayout.Toggle(DisableBToMoveUpDirectory, new GUIContent("Disable B/O Button to Move Up Directory"));
            EnableDarkMode = GUILayout.Toggle(EnableDarkMode, new GUIContent("Enable Dark Mode"));
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

    public enum FontSizePreset
    {
        Normal = 0,
        Small = 1,
        Smaller = 2,
    }
}
