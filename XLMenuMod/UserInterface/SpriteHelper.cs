using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using XLMenuMod.Interfaces;

namespace XLMenuMod.UserInterface
{
	public class SpriteHelper
	{
		public static void LoadCustomFolderSprite(ICustomFolderInfo folder, string path)
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(Path.Combine(path, "folder.png"))) return;

			var folderIconPath = Path.Combine(path, "folder.png");

			// Assign new Sprite Sheet texture to the Sprite Asset.
			var texture = new Texture2D(2, 2);
			texture.LoadImage(File.ReadAllBytes(folderIconPath));

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
			sprite.width = 256;
			sprite.height = 256;
			sprite.pivot = new Vector2(0, 0);
			//sprite.x = importedSprites[i].frame.x;
			//sprite.y = m_SpriteAtlas.height - (importedSprites[i].frame.y + importedSprites[i].frame.h);
			//sprite.width = importedSprites[i].frame.w;
			//sprite.height = importedSprites[i].frame.h;

			////Calculate sprite pivot position
			//sprite.pivot = importedSprites[i].pivot;

			// Properties the can be modified
			sprite.xAdvance = sprite.width;
			sprite.scale = 1.0f;
			sprite.xOffset = 0 - (sprite.width * sprite.pivot.x);
			sprite.yOffset = sprite.height - (sprite.height * sprite.pivot.y);

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
		/// <param name="FilePath"></param>
		/// <returns>Null if load fails</returns>
		public Texture2D LoadTexture(string FilePath)
		{
			Texture2D texture;
			byte[] data;

			if (File.Exists(FilePath))
			{
				data = File.ReadAllBytes(FilePath);
				texture = new Texture2D(2, 2);
				if (texture.LoadImage(data))
					return texture;
			}

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
	}
}
