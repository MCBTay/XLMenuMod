using HarmonyLib;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
using XLMenuMod.Gear;
using XLMenuMod.Levels;

namespace XLMenuMod
{
    static class Main
    {
        public static bool Enabled { get; private set; }

        private static Harmony Harmony { get; set; }
        public static string ModPath { get; private set; }

        public static GameObject CustomLevelManagerGameObject;
        public static GameObject CustomGearManagerGameObject;

        public static Settings Settings { get; private set; }

        public static AssetBundle Assets { get; private set; }
        public static TMP_SpriteAsset BlackSprites { get; private set; }
        public static TMP_SpriteAsset BlueSprites { get; private set; }

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

                CustomLevelManagerGameObject = new GameObject();
                CustomLevelManagerGameObject.AddComponent<CustomLevelManager>();
                Object.DontDestroyOnLoad(CustomLevelManagerGameObject);

                CustomGearManagerGameObject = new GameObject();
                CustomGearManagerGameObject.AddComponent<CustomGearManager>();
                Object.DontDestroyOnLoad(CustomGearManagerGameObject);

                Assets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.xlmenumod"));

                var spriteAssets = Assets.LoadAllAssets<TMP_SpriteAsset>();
                if (spriteAssets != null)
                {
                    BlackSprites = spriteAssets.First();
                    BlueSprites = spriteAssets.ElementAt(1);
                }

                // Replace CustomLevelsCache.json with saved version if it exists. 
                // This is to prevent unnecessary hashes.
                if (File.Exists(Path.Combine(ModPath, "CachedCustomLevels.json")))
                {
                    File.Copy(Path.Combine(ModPath, "CachedCustomLevels.json"), SaveManager.Instance.CachedLevelsPath, true);
                }
                CustomLevelManager.Instance.LoadNestedItems();
            }
            else
            {
                Assets?.Unload(true);

                Harmony.UnpatchAll(Harmony.Id);

                Object.Destroy(CustomLevelManagerGameObject.GetComponent<CustomLevelManager>());
                Object.Destroy(CustomGearManagerGameObject.GetComponent<CustomGearManager>());

                // TODO: At this point, we likely need to revert the custom levels to their original.
            }

            return true;
        }

        private static byte[] ExtractResource(string filename)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (var resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }  
}
