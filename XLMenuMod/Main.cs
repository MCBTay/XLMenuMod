using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
using XLMenuMod.Gear;
using XLMenuMod.Levels;
using Object = UnityEngine.Object;

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
        public static List<TMP_SpriteAsset> Sprites { get; private set; }

        public static TMP_SpriteAsset WhiteSprites => Sprites?.ElementAt(2);

        public static AssetBundle BrandAssets { get; private set; }
        public static TMP_SpriteAsset BrandSprites { get; private set; }

        public static Sprite DarkModeBackground { get; private set; }

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
				Sprites = LoadSpriteSheet(Assets).ToList();

                BrandAssets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.spritesheets_brands"));
                BrandSprites = LoadSpriteSheet(BrandAssets).FirstOrDefault();

                Assets.Unload(false);
                BrandAssets.Unload(false);

                LoadBackgroundTexture();
            }
            else
            {
	            Harmony.UnpatchAll(Harmony.Id);

                Object.Destroy(CustomLevelManagerGameObject.GetComponent<CustomLevelManager>());
                Object.Destroy(CustomGearManagerGameObject.GetComponent<CustomGearManager>());
            }

            return true;
        }

        private static List<TMP_SpriteAsset> LoadSpriteSheet(AssetBundle bundle)
        {
	        var spriteBrandAssets = bundle.LoadAllAssets<TMP_SpriteAsset>();
	        if (spriteBrandAssets != null)
	        {
		        return spriteBrandAssets.ToList();
	        }

	        return null;
        }

        public static byte[] ExtractResource(string filename)
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

        

        public static void LoadBackgroundTexture()
        {
	        var texture2d = new Texture2D(2, 2);
	        if (!texture2d.LoadImage(Main.ExtractResource("XLMenuMod.Assets.darkmode.png"))) return;

	        Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
	        DarkModeBackground = sprite;
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }
    }  
}
