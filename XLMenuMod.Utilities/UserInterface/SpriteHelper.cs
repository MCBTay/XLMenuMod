﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Utilities.UserInterface
{
	public class SpriteHelper
	{
		public AssetBundle XLMenuModAB { get; set; }

		public static TMP_SpriteAsset MenuIcons { get; private set; }
		public static TMP_SpriteAsset BrandIcons { get; private set; }
		public static Sprite OriginalBackground { get; set; }
		public static Sprite DarkModeBackground { get; private set; }
		public static Texture2D DarkModeReplayBackground { get; private set; }

		public static TMP_SpriteAsset DarkControllerIcons { get; private set; }
		public static TMP_SpriteAsset LightControllerIcons { get; private set; }

		private static SpriteHelper _instance;
		public static SpriteHelper Instance
		{
			get { return _instance ??= new SpriteHelper(); }
			private set { _instance = value; }
		}

		public SpriteHelper()
		{
			Instance = this;
		}

		public void LoadSprites()
		{
			XLMenuModAB = AssetBundle.LoadFromMemory(UserInterfaceHelper.ExtractResource("XLMenuMod.Utilities.Assets.xlmenumod"));
			MenuIcons = XLMenuModAB.LoadAsset<TMP_SpriteAsset>("UISpriteSheet_White");
			BrandIcons = XLMenuModAB.LoadAsset<TMP_SpriteAsset>("BrandsSpriteSheet_White");
			DarkModeBackground = XLMenuModAB.LoadAsset<Sprite>("DarkModeBackground");
			DarkModeReplayBackground = XLMenuModAB.LoadAsset<Texture2D>("DarkModeReplayBackground");
			XLMenuModAB.Unload(false);

			var spriteAssets = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>();
			DarkControllerIcons = spriteAssets.FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_Greyish");
			LightControllerIcons = spriteAssets.FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_White");
		}

		//TODO: Currently this doesn't work on a folder that has no textures in it.
		public void LoadCustomFolderSprite(ICustomFolderInfo folder, string path)
		{
			if (string.IsNullOrEmpty(path)) return;

			var folderIconPath = Path.Combine(path, "folder.png");

			if (!File.Exists(folderIconPath))
			{
				folderIconPath = Path.Combine(path, "folder.jpg");
			}

			if (!File.Exists(folderIconPath))
			{
				folderIconPath = Path.Combine(path, "folder.jpeg");
			}

			if (!File.Exists(folderIconPath)) return;

			// Assign new Sprite Sheet texture to the Sprite Asset.
			var texture = LoadTexture(folderIconPath);

			List<TMP_Sprite> spriteInfoList = new List<TMP_Sprite>();

			TMP_Sprite sprite = new TMP_Sprite();

			sprite.id = 0;
			sprite.name = Path.GetFileNameWithoutExtension(folderIconPath) ?? "";
			sprite.hashCode = TMP_TextUtilities.GetSimpleHashCode(sprite.name);

			//// Attempt to extract Unicode value from name
			int unicode;
			int indexOfSeperator = sprite.name.IndexOf('-');
			if (indexOfSeperator != -1)
				unicode = TMP_TextUtilities.StringHexToInt(sprite.name.Substring(indexOfSeperator + 1));
			else
				unicode = TMP_TextUtilities.StringHexToInt(sprite.name);

			sprite.unicode = unicode;

			sprite.x = 0;
			sprite.y = 0;
			sprite.width = texture.width;
			sprite.height = texture.height;
			sprite.pivot = new Vector2(0.5f, 0.5f);

			sprite.xAdvance = sprite.width;
			sprite.scale = 1.0f;
			sprite.xOffset = 0 - (sprite.width * sprite.pivot.x);
			sprite.yOffset = sprite.height;

			spriteInfoList.Add(sprite);

			// Create new Sprite Asset using this texture
			TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
			// Compute the hash code for the sprite asset.
			spriteAsset.hashCode = TMP_TextUtilities.GetSimpleHashCode(spriteAsset.name);

			spriteAsset.spriteSheet = texture;
			spriteAsset.spriteInfoList = spriteInfoList;

			// Add new default material for sprite asset.
			AddDefaultMaterial(spriteAsset);

			folder.CustomSprite = spriteAsset;
		}

		/// <summary>
		/// Load a PNG or JPG file from disk to a Texture2D
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>Null if load fails</returns>
		public Texture2D LoadTexture(string filePath)
		{
            if (!File.Exists(filePath)) return null;

            var data = File.ReadAllBytes(filePath);
            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(data)) return null;

            if (texture.width <= 256 && texture.height <= 256) return texture;

            UnityModManager.Logger.Log($"XLMenuMod: {filePath} is too large.  Max dimensions are 256x256.");
            Object.Destroy(texture);
            return null;

        }

		/// <summary>
		/// Create and add new default material to sprite asset.
		/// </summary>
		/// <param name="spriteAsset"></param>
		static void AddDefaultMaterial(TMP_SpriteAsset spriteAsset)
		{
			Shader shader = Shader.Find("TextMeshPro/Sprite");
			Material material = new Material(shader);
			material.SetTexture(ShaderUtilities.ID_MainTex, spriteAsset.spriteSheet);

			spriteAsset.material = material;
			material.hideFlags = HideFlags.HideInHierarchy;
		}

		public int GetSpriteIndex_YButton_Gray(RuntimePlatform platform, string joystickName = "unknown")
		{
			ControllerIconSpriteGray returnVal;

			switch (platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = joystickName ?? "unknown";
                    if (str.Contains("Dual Shock") || str.Contains("DualShock"))
                    {
                        returnVal = ControllerIconSpriteGray.PS4_Triangle_Button;
                        break;
                    }
                    returnVal = ControllerIconSpriteGray.XB1_Y;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSpriteGray.PS4_Triangle_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSpriteGray.SWITCH_X;
					break;
				default:
					returnVal = ControllerIconSpriteGray.XB1_Y;
					break;
			}

			return (int)returnVal;
		}

		public int GetSpriteIndex_YButton(RuntimePlatform platform, string joystickName = "unknown")
		{
			ControllerIconSprite returnVal;

			switch (platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = joystickName ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
                    {
                        returnVal = ControllerIconSprite.PS4_Triangle_Button;
                        break;
                    }
                    returnVal = ControllerIconSprite.XB1_Y;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Triangle_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_X;
					break;
				default:
					returnVal = ControllerIconSprite.XB1_Y;
					break;
			}

			return (int)returnVal;
		}

		public int GetSpriteIndex_XButton(RuntimePlatform platform, string joystickName = "unknown")
		{
			ControllerIconSprite returnVal;

			switch (platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
                    string str = joystickName ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						returnVal = ControllerIconSprite.PS4_Square_Button;
						break;
					}
					returnVal = ControllerIconSprite.XB1_X;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Square_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_Y;
					break;
				default:
					returnVal = ControllerIconSprite.XB1_X;
					break;
			}

			return (int)returnVal;
		}

		
	}
}
