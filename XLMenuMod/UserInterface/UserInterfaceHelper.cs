using GameManagement;
using HarmonyLib;
using ReplayEditor;
using Rewired;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLMenuMod.UserInterface
{
	public class UserInterfaceHelper
    {
	    public AssetBundle Assets { get; private set; }
	    public TMP_SpriteAsset Sprites { get; private set; }

	    public AssetBundle BrandAssets { get; private set; }
	    public TMP_SpriteAsset BrandSprites { get; private set; }

		public static Sprite OriginalBackground { get; set; }
	    public static Sprite DarkModeBackground { get; set; }

		public static Texture2D OriginalReplayBackground { get; set; }
		public static Color OriginalReplayHeaderColor = new Color(0.973f, 0.973f, 0.973f, 0.600f);
		public static Color DarkModeReplayHeaderColor = new Color(63f / 255f, 63f / 255f, 63f / 255f, 0.8f);
		public static Texture2D DarkModeReplayBackground { get; set; }
		public static Color32 DarkModeTextColor = new Color32(244, 245, 245, 255);
		public static Color32 BlueAccentColor = new Color(0.204f, 0.541f, 0.961f, 1.000f);

		public static TMP_SpriteAsset DarkControllerIcons { get; set; }
		public static TMP_SpriteAsset LightControllerIcons { get; set; }

		private static UserInterfaceHelper _instance;
	    public static UserInterfaceHelper Instance
	    {
		    get { return _instance ?? (_instance = new UserInterfaceHelper()); }
		    private set { _instance = value; }
	    }

		public UserInterfaceHelper()
		{
			Instance = this;
		}

		public void LoadAssets()
		{
			Assets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.xlmenumod"));
			Sprites = Assets.LoadAllAssets<TMP_SpriteAsset>()?.FirstOrDefault();
			Assets.Unload(false);

			BrandAssets = AssetBundle.LoadFromMemory(ExtractResource("XLMenuMod.Assets.spritesheets_brands"));
			BrandSprites = BrandAssets.LoadAllAssets<TMP_SpriteAsset>()?.FirstOrDefault();
			BrandAssets.Unload(false);

			LoadBackgroundTexture();

			var spriteAssets = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>();
			DarkControllerIcons = spriteAssets.FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_Greyish");
			LightControllerIcons = spriteAssets.FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_White");
		}

		

		public TMP_Text CreateSortLabel(TMP_Text sourceText, Transform parent, string sort, int yOffset = -50)
        {
            TMP_Text label = GameObject.Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);
            
            UpdateLabelColor(label, Main.Settings.EnableDarkMode ? DarkModeText : DefaultText);
            label.spriteAsset = Main.Settings.EnableDarkMode ? LightControllerIcons : DarkControllerIcons;

            label.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.gameObject.SetActive(false);

            SetSortLabelText(ref label, sort.Replace('_', ' '));

            label.transform.localPosition = Vector3.zero;
            label.transform.Translate(new Vector3(0, yOffset, 0));

            return label;
        }

        public void SetSortLabelText(ref TMP_Text label, string text)
        {
            var sortLabelText = $"<size=80%><sprite={SpriteHelper.GetSpriteIndex_YButton_Gray()}> <size=60%><b>Sort By:</b> " + text.Replace('_', ' ');
            //var defaultLabelText = $"<size=60%><voffset=0.25em><sprite={GetSpriteIndex_XButton()}></voffset> <b>Set Default</b>";

            label?.SetText(sortLabelText); //+ defaultLabelText);
        }

        private void LoadBackgroundTexture()
        {
	        var menuBackground = new Texture2D(2, 2);
	        if (!menuBackground.LoadImage(ExtractResource("XLMenuMod.Assets.darkmode.png"))) return;

	        DarkModeBackground = Sprite.Create(menuBackground, new Rect(0, 0, menuBackground.width, menuBackground.height), new Vector2(0.5f, 0.5f));

			DarkModeReplayBackground = new Texture2D(2, 2);
	        if (!DarkModeReplayBackground.LoadImage(ExtractResource("XLMenuMod.Assets.PanelTransparent.png"))) return;
	        DarkModeReplayBackground.name = "PanelTransparent";
        }

        public static ColorBlock DarkModeText = new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = DarkModeTextColor,
			fadeDuration = 0,
			highlightedColor = DarkModeTextColor,
			//highlightedColor = new Color(0.973f, 0.973f, 0.973f, 1.000f),
			normalColor = DarkModeTextColor,
			pressedColor = DarkModeTextColor,
			selectedColor = DarkModeTextColor
		};

		public static ColorBlock DarkModeSliderText = new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = DarkModeTextColor,
			fadeDuration = 0,
			highlightedColor = BlueAccentColor,
			normalColor = DarkModeTextColor,
			pressedColor = DarkModeTextColor,
			selectedColor = BlueAccentColor
		};

		public static ColorBlock DefaultText = new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = new Color(0.784f, 0.784f, 0.784f, 1.000f),
			fadeDuration = 0,
			highlightedColor = new Color(0.973f, 0.973f, 0.973f, 1.000f),
			normalColor = new Color(0.267f, 0.267f, 0.267f, 1.000f),
			pressedColor = new Color(0.784f, 0.784f, 0.784f, 1.000f),
			selectedColor = new Color(0.973f, 0.973f, 0.973f, 1.000f)
		};

		public static ColorBlock DefaultSliderText = new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = new Color(0.784f, 0.784f, 0.784f, .502f),
			fadeDuration = 0,
			highlightedColor = BlueAccentColor,
			normalColor = new Color(0.267f, 0.267f, 0.267f, 1.000f),
			pressedColor = new Color(0.784f, 0.784f, 0.784f, 1.000f),
			selectedColor = BlueAccentColor
		};
		
		public void UpdateLabelColor(Selectable button, ColorBlock color)
        {
	        button.colors = color;
        }

		public void UpdateLabelColor(MVCListHeaderView header, ColorBlock color)
		{
			header.Label.color = color.normalColor;
		}

		public void UpdateLabelColor(TMP_Text label, ColorBlock color)
		{
			var blue = new Color(0.007843138f, 0.4509804f, 1.000f, 1.000f);
			
			if (Mathf.Approximately(label.color.r, blue.r) &&
				Mathf.Approximately(label.color.g, blue.g) &&
				Mathf.Approximately(label.color.b, blue.b) &&
				Mathf.Approximately(label.color.a, blue.a)) return;

			label.color = color.normalColor;
		}

        public void UpdateFontSize(TMP_Text label)
        {
	        switch (Main.Settings.FontSize)
	        {
		        case FontSizePreset.Small:
			        label.fontSize = 30;
			        break;
		        case FontSizePreset.Smaller:
			        label.fontSize = 24;
			        break;
		        case FontSizePreset.Normal:
		        default:
			        label.fontSize = 36;
			        break;
	        }
        }

        public void ToggleDarkMode(bool enabled)
        {
			GameStateMachine.Instance.PauseObject.ToggleDarkMode(enabled);
			GameStateMachine.Instance.SettingsObject.ToggleDarkMode(enabled);

			ToggleDarkMode(GameStateMachine.Instance.TutorialMenuObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.FeetControlTutorialObject, enabled, true);
			ToggleDarkMode(GameStateMachine.Instance.TricksMenuObject, enabled, true);
			ToggleDarkMode(GameStateMachine.Instance.TutorialFlowObject, enabled, true);

			ToggleDarkMode(GameStateMachine.Instance.ChallengeSummaryObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.ChallengePlayObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.SpotSelectionObject, enabled);

			GameStateMachine.Instance.LevelSelectionObject.ToggleDarkMode(enabled);

			GameStateMachine.Instance.ReplayMenuObject.ToggleDarkMode(enabled, true, true);
			GameStateMachine.Instance.ReplayDeleteDialog.ToggleDarkMode(enabled, true);
			ReplayEditorController.Instance.ReplayUI.ToggleDarkMode(enabled, true, true);
			ReplayEditorController.Instance.SaveMenu.gameObject.ToggleDarkMode(enabled, true, true);
        }

        public void ToggleDarkMode(GameObject gameObject, bool enabled, bool hasStaticText = false, bool hasSubmeshes = false)
        {
	        if (gameObject == null) return;

	        SetBackgroundTexture(gameObject);

	        if (hasStaticText)
	        {
		        var labels = gameObject.GetComponentsInChildren<TMP_Text>(true);
		        if (labels != null)
		        {
			        foreach (var label in labels)
			        {
				        UpdateLabelColor(label, enabled ? DarkModeText : DefaultText);

				        if (label.text.Contains("<sprite"))
				        {
					        if (label.spriteAsset.name.Contains("Controller"))
					        {
						        label.spriteAsset = enabled ? LightControllerIcons : DarkControllerIcons;
					        }
				        }
					}
		        }
	        }

	        if (hasSubmeshes)
	        {
		        var submeshes = gameObject.GetComponentsInChildren<TMP_SubMeshUI>(true);
		        if (submeshes != null)
		        {
			        foreach (var controllerButton in submeshes)
			        {
				        if (controllerButton?.spriteAsset != null && controllerButton?.material != null)
				        {
					        controllerButton.material.color = enabled ? DarkModeText.normalColor : DefaultText.normalColor;
				        }
			        }
		        }
			}

	        ToggleDarkMode<MenuButton>(gameObject, enabled);
			ToggleDarkMode<MenuSlider>(gameObject, enabled);
			ToggleDarkMode<MenuToggle>(gameObject, enabled);

			var comps = GameStateMachine.Instance.ChallengeSummaryObject.GetComponentInChildren<ChallengeSummaryController>();
			if (comps != null)
			{
				var text = comps.FailureHeader.GetComponentInChildren<TMP_Text>();
				UpdateLabelColor(text, enabled ? DarkModeText : DefaultText);
			}

			var trickList = gameObject.GetComponentInChildren<ChallengeTrickListController>();
			if (trickList != null)
			{
				var trickListItemViews = Traverse.Create<ChallengeTrickListController>().Field<List<ChallengeTrickItemView>>("ItemViews").Value;
				if (trickListItemViews != null && trickListItemViews.Any())
				{
					trickListItemViews.ToggleDarkMode(enabled);
				}
			}

			var listView = gameObject.GetComponentInChildren<MVCListView>();
			ToggleDarkMode(listView, enabled);
			
			// This is for the replay editor UIs
			var images = gameObject.GetComponentsInChildren<Image>(true);
			foreach (var image in images)
			{
				if (image.mainTexture.name == "PanelTransparent")
				{
					if (OriginalReplayBackground == null)
					{
						OriginalReplayBackground = image.mainTexture as Texture2D;
					}

					var texture = enabled ? DarkModeReplayBackground : OriginalReplayBackground;
					var newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(72f, 72f), 300, 0, SpriteMeshType.Tight, new Vector4(30f, 34f, 29f, 27f));
					image.sprite = newSprite;
				}
				else if (image.mainTexture.name == "UnityWhite")
				{
					image.color = enabled ? DarkModeReplayHeaderColor : OriginalReplayHeaderColor;
				}
			}
		}

        public void ToggleDarkMode(MVCListView listView, bool enabled)
        {
	        if (listView == null) return;

	        UpdateFontSize(listView.ItemPrefab.Label);

	        ToggleDarkMode(listView.ItemPrefab, enabled);
	        UpdateLabelColor(listView.HeaderView, enabled ? DarkModeText : DefaultText);
	        foreach (var item in listView.ItemViews)
	        {
		        ToggleDarkMode(item, enabled);
	        }
		}

        public void ToggleDarkMode(MVCListItemView listItemView, bool enabled)
        {
	        var textColor = enabled ? DarkModeText : DefaultText;

	        UpdateFontSize(listItemView.Label);
	        UpdateLabelColor(listItemView, textColor);
		}

        private void ToggleDarkMode<T>(GameObject gameObject, bool enabled) where T : Selectable
        {
	        if (gameObject == null) return;

	        var controls = gameObject.GetComponentsInChildren<T>(true);
	        foreach (var control in controls)
	        {
		        if (control is MenuSlider menuSlider)
		        {
			        if (menuSlider.selectionIndicator == null)
			        {
				        UpdateLabelColor(menuSlider, enabled ? DarkModeSliderText : DefaultSliderText);
					}
			        else
			        {
						UpdateLabelColor(menuSlider, enabled ? DarkModeText : DefaultText);
					}
		        }
		        else
		        {
					UpdateLabelColor(control, enabled ? DarkModeText : DefaultText);
				}
	        }
		}

        private void SetBackgroundTexture(GameObject gameObject)
        {
	        if (gameObject == null) return;

	        var textures = gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "MenuPanelBackground");

	        if (textures != null)
	        {
		        if (OriginalBackground == null)
		        {
			        OriginalBackground = textures.sprite;
		        }

		        if (Main.Settings.EnableDarkMode && DarkModeBackground != null)
		        {
			        textures.sprite = DarkModeBackground;
		        }
		        else
		        {
			        textures.sprite = OriginalBackground;
		        }
	        }
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

	public static class Extensions
	{
		public static void ToggleDarkMode(this List<ChallengeTrickItemView> trickList, bool enabled)
		{
			foreach (var trick in trickList)
			{
				UserInterfaceHelper.Instance.UpdateLabelColor(trick.label, enabled ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText);
			}
		}
	}
}
