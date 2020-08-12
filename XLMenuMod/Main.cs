using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using XLMenuMod.UserInterface;

namespace XLMenuMod
{
	static class Main
    {
        public static bool Enabled { get; private set; }

        private static Harmony Harmony { get; set; }
        public static string ModPath { get; private set; }

        public static Settings Settings { get; private set; }

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = Settings.OnSettingsGUI;
            modEntry.OnSaveGUI = OnSaveGUI;

            ModPath = modEntry.Path;

            return true;
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (Enabled == value) return true;
            Enabled = value;

            if (Enabled)
            { 
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());

                UserInterfaceHelper.Instance.LoadAssets();
                UserInterfaceHelper.Instance.ToggleDarkMode(Settings.EnableDarkMode);
            }
            else
            {
                UserInterfaceHelper.Instance.ToggleDarkMode(false);

                Harmony.UnpatchAll(Harmony.Id);
            }

            return true;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }  
}
