using Harmony12;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XLMenuMod.Gear;
using XLMenuMod.Levels;

namespace XLMenuMod
{
    static class Main
    {
        public static bool Enabled { get; private set; }

        private static HarmonyInstance Harmony { get; set; }

        public static GameObject CustomLevelManagerGameObject;
        public static GameObject CustomGearManagerGameObject;

        public static Settings Settings { get; private set; }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = Settings.OnSettingsGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (Enabled == value) return true;
            Enabled = value;

            if (Enabled)
            { 
                Harmony = HarmonyInstance.Create(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());

                CustomLevelManagerGameObject = new GameObject();
                CustomLevelManagerGameObject.AddComponent<CustomLevelManager>();
                Object.DontDestroyOnLoad(CustomLevelManagerGameObject);

                CustomGearManagerGameObject = new GameObject();
                CustomGearManagerGameObject.AddComponent<CustomGearManager>();
                Object.DontDestroyOnLoad(CustomGearManagerGameObject);

                CustomLevelManager.LoadNestedLevels();
            }
            else
            {
                Harmony.UnpatchAll(Harmony.Id);

                Object.Destroy(CustomLevelManagerGameObject.GetComponent<CustomLevelManager>());
                Object.Destroy(CustomGearManagerGameObject.GetComponent<CustomGearManager>());
            }

            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }  
}
