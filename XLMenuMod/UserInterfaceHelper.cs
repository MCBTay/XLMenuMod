using System.Collections.Generic;
using Rewired;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace XLMenuMod
{
	public class UserInterfaceHelper
    {
	    public AssetBundle Assets { get; private set; }
	    public List<TMP_SpriteAsset> Sprites { get; private set; }

	    public TMP_SpriteAsset WhiteSprites => Sprites?.ElementAt(2);

	    public AssetBundle BrandAssets { get; private set; }
	    public TMP_SpriteAsset BrandSprites { get; private set; }

	    public Sprite DarkModeBackground { get; private set; }

		public static UserInterfaceHelper Instance { get; set; }

		public UserInterfaceHelper()
		{
			Instance = this;
		}

		public void LoadAssets()
		{
			Assets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.xlmenumod"));
			Sprites = LoadSpriteSheet(Assets).ToList();

			BrandAssets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.spritesheets_brands"));
			BrandSprites = LoadSpriteSheet(BrandAssets).FirstOrDefault();

			Assets.Unload(false);
			BrandAssets.Unload(false);

			LoadBackgroundTexture();
		}

		public TMP_Text CreateSortLabel(TMP_Text sourceText, Transform parent, string sort, int yOffset = -50)
        {
            TMP_Text label = GameObject.Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);
            label.color = Color.black;

            var controllerIcons = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>().FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_Greyish");
            if (controllerIcons != null)
            {
                label.spriteAsset = controllerIcons;
            }

            label.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.gameObject.SetActive(false);

            SetSortLabelText(ref label, sort.Replace('_', ' '));

            label.transform.localPosition = Vector3.zero;
            label.transform.Translate(new Vector3(0, yOffset, 0));

            return label;
        }

        public void SetSortLabelText(ref TMP_Text label, string text)
        {
            var sortLabelText = $"<size=80%><sprite={GetSpriteIndex_YButton_Gray()}> <size=60%><b>Sort By:</b> " + text.Replace('_', ' ');
            //var defaultLabelText = $"<size=60%><voffset=0.25em><sprite={GetSpriteIndex_XButton()}></voffset> <b>Set Default</b>";

            label?.SetText(sortLabelText); //+ defaultLabelText);
        }

        public int GetSpriteIndex_YButton_Gray()
        {
	        ControllerIconSprite_Gray returnVal;

	        switch (Application.platform)
	        {
		        case RuntimePlatform.WindowsPlayer:
		        case RuntimePlatform.WindowsEditor:
			        string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
			        if (str.Contains("Dual Shock") || str.Contains("DualShock"))
			        {
				        returnVal = ControllerIconSprite_Gray.PS4_Triangle_Button;
				        break;
			        }
			        returnVal = ControllerIconSprite_Gray.XB1_Y;
			        break;
		        case RuntimePlatform.PS4:
			        returnVal = ControllerIconSprite_Gray.PS4_Triangle_Button;
			        break;
		        case RuntimePlatform.Switch:
			        returnVal = ControllerIconSprite_Gray.SWITCH_X;
			        break;
		        case RuntimePlatform.XboxOne:
		        default:
			        returnVal = ControllerIconSprite_Gray.XB1_Y;
			        break;
	        }

	        return (int)returnVal;
        }

        public int GetSpriteIndex_YButton()
        {
	        ControllerIconSprite returnVal;

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
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
                case RuntimePlatform.XboxOne:
                default:
	                returnVal = ControllerIconSprite.XB1_Y;
	                break;
            }

            return (int)returnVal;
        }

        public int GetSpriteIndex_XButton()
        {
	        ControllerIconSprite returnVal;

	        switch (Application.platform)
	        {
		        case RuntimePlatform.WindowsPlayer:
		        case RuntimePlatform.WindowsEditor:
			        string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
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
			        returnVal = ControllerIconSprite.SWITCH_X;
			        break;
		        case RuntimePlatform.XboxOne:
		        default:
			        returnVal = ControllerIconSprite.XB1_X;
			        break;
	        }

	        return (int)returnVal;
        }

        private void LoadBackgroundTexture()
        {
	        var texture2d = new Texture2D(2, 2);
	        if (!texture2d.LoadImage(ExtractResource("XLMenuMod.Assets.darkmode.png"))) return;

	        Sprite sprite = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
	        DarkModeBackground = sprite;
        }

        private List<TMP_SpriteAsset> LoadSpriteSheet(AssetBundle bundle)
        {
	        var spriteBrandAssets = bundle.LoadAllAssets<TMP_SpriteAsset>();
	        if (spriteBrandAssets != null)
	        {
		        return spriteBrandAssets.ToList();
	        }

	        return null;
        }

        private byte[] ExtractResource(string filename)
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
	}
}
