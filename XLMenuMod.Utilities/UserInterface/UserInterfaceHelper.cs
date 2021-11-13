using GameManagement;
using ReplayEditor;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLMenuMod.Utilities.UserInterface
{
    public class UserInterfaceHelper
    {
	    public static Texture2D OriginalReplayBackground { get; set; }
		public static Color OriginalReplayHeaderColor => new Color(0.973f, 0.973f, 0.973f, 0.600f);

		public static Color DarkModeReplayHeaderColor => new Color(63f / 255f, 63f / 255f, 63f / 255f, 0.8f);

		public static Color32 DarkModeTextColor => new Color32(244, 245, 245, 255);
		public static Color32 BlueAccentColor => new Color(0.204f, 0.541f, 0.961f, 1.000f);

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
			SpriteHelper.Instance.LoadSprites();
		}

		public TMP_Text CreateSortLabel(bool darkModeEnabled, TMP_Text sourceText, Transform parent, string sort, int yOffset = -50)
        {
            TMP_Text label = GameObject.Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);
            
            UpdateLabelColor(label, darkModeEnabled ? DarkModeText : DefaultText);
            label.spriteAsset = darkModeEnabled ? SpriteHelper.LightControllerIcons : SpriteHelper.DarkControllerIcons;

            label.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.gameObject.SetActive(false);
			   
            SetSortLabelText(ref label, sort.Replace('_', ' '));

            label.transform.localPosition = Vector3.zero;
            label.transform.Translate(new Vector3(0, yOffset, 0));

            return label;
        }

        public void SetSortLabelText(ref TMP_Text label, string text)
        {
			var sortLabelText = $"<size=80%><sprite={SpriteHelper.Instance.GetSpriteIndex_YButton_Gray(Application.platform)}> <size=60%><b>Sort By:</b> " + text.Replace('_', ' ');
            label?.SetText(sortLabelText);
        }

        public static ColorBlock DarkModeText => new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = DarkModeTextColor,
			fadeDuration = 0,
			highlightedColor = DarkModeTextColor,
			normalColor = DarkModeTextColor,
			pressedColor = DarkModeTextColor,
			selectedColor = DarkModeTextColor
		};

		public static ColorBlock DarkModeSliderText => new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = DarkModeTextColor,
			fadeDuration = 0,
			highlightedColor = BlueAccentColor,
			normalColor = DarkModeTextColor,
			pressedColor = DarkModeTextColor,
			selectedColor = BlueAccentColor
		};

		public static ColorBlock DefaultText => new ColorBlock
		{
			colorMultiplier = 1,
			disabledColor = new Color(0.267f, 0.267f, 0.267f, 1.000f),
			fadeDuration = 0,
			highlightedColor = new Color(0.973f, 0.973f, 0.973f, 1.000f),
			normalColor = new Color(0.267f, 0.267f, 0.267f, 1.000f),
			pressedColor = new Color(0.784f, 0.784f, 0.784f, 1.000f),
			selectedColor = new Color(0.973f, 0.973f, 0.973f, 1.000f)
		};

		public static ColorBlock DefaultSliderText => new ColorBlock
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

		public void UpdateLabelColor(TMP_Text label, ColorBlock color)
		{
			var blue = new Color(0.007843138f, 0.4509804f, 1.000f, 1.000f);
			
			if (Mathf.Approximately(label.color.r, blue.r) &&
				Mathf.Approximately(label.color.g, blue.g) &&
				Mathf.Approximately(label.color.b, blue.b) &&
				Mathf.Approximately(label.color.a, blue.a)) return;

			label.color = color.normalColor;
		}

		//TODO: Come back to this!!
        public void UpdateFontSize(TMP_Text label)
        {
	        //switch (Main.Settings.FontSize)
	        //{
		       // case FontSizePreset.Small:
			      //  label.fontSize = 30;
			      //  break;
		       // case FontSizePreset.Smaller:
			      //  label.fontSize = 24;
			      //  break;
		       // case FontSizePreset.Normal:
		       // default:
			      //  label.fontSize = 36;
			      //  break;
	        //}
        }

        public void ToggleDarkMode(bool enabled)
        {
	        ToggleDarkMode(GameStateMachine.Instance.PauseObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.SettingsObject, enabled);

			ToggleDarkMode(GameStateMachine.Instance.TutorialMenuObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.FeetControlTutorialObject, enabled, true);
			ToggleDarkMode(GameStateMachine.Instance.TricksMenuObject, enabled, true);
			ToggleDarkMode(GameStateMachine.Instance.TutorialFlowObject, enabled, true);

			ToggleDarkMode(GameStateMachine.Instance.ChallengeSummaryObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.ChallengePlayObject, enabled);
			ToggleDarkMode(GameStateMachine.Instance.SpotSelectionObject, enabled);

			ToggleDarkMode(GameStateMachine.Instance.LevelSelectionObject, enabled);

			ToggleDarkMode(GameStateMachine.Instance.ReplayMenuObject, enabled, true, true);
			ToggleDarkMode(GameStateMachine.Instance.ReplayDeleteDialog, enabled, true);
			ToggleDarkMode(ReplayEditorController.Instance.ReplayUI, enabled, true, true);
			ToggleDarkMode(ReplayEditorController.Instance.Menu.SaveMenu.gameObject, enabled, true, true);
			ToggleDarkMode(ReplayEditorController.Instance.Menu.MainMenuPanel.gameObject, enabled, true, true);
			ToggleDarkMode(ReplayEditorController.Instance.Menu.SettingsMenu.gameObject, enabled, true, true);

			ToggleDarkMode(GameStateMachine.Instance.ModBrowserObject, enabled, true, true);

			ToggleDarkMode(GameStateMachine.Instance.WeblinksMenu, enabled, true);

			ToggleDarkMode(GameStateMachine.Instance.MultiplayerMenuObject, enabled, true);
        }

        public void ToggleDarkMode(GameObject gameObject, bool enabled, bool hasStaticText = false, bool hasSubmeshes = false)
        {
	        if (gameObject == null) return;

	        SetBackgroundTexture(gameObject, enabled);

	        ToggleDarkModeForStaticText(gameObject, hasStaticText, enabled);
            ToggleDarkModeForSubmeshes(gameObject, hasSubmeshes, enabled);

            gameObject.GetComponentsInChildren<MenuButton>(true).ToggleDarkMode(enabled);
            gameObject.GetComponentsInChildren<MenuSlider>(true).ToggleDarkMode(enabled);
            gameObject.GetComponentsInChildren<MenuToggle>(true).ToggleDarkMode(enabled);

			gameObject.GetComponentInChildren<ChallengeSummaryController>(true).ToggleDarkMode(enabled);
            gameObject.GetComponentInChildren<ChallengeTrickListController>(true).ToggleDarkMode(enabled);
			gameObject.GetComponentInChildren<MVCListView>(true).ToggleDarkMode(enabled);
            gameObject.GetComponentsInChildren<Image>(true).ToggleDarkMode(enabled);
        }

        private void ToggleDarkModeForStaticText(GameObject gameObject, bool hasStaticText, bool enabled)
        {
            if (!hasStaticText) return;

            var labels = gameObject.GetComponentsInChildren<TMP_Text>(true);
            if (labels == null) return;

            foreach (var label in labels)
            {
                if (label.transform.parent.parent.name == "ListViewHeader") continue;
                if (label.transform.parent.gameObject.GetComponent<MenuButton>() != null) continue;

                UpdateLabelColor(label, enabled ? DarkModeText : DefaultText);

                if (label.text.Contains("<sprite") && label.spriteAsset.name.Contains("Controller"))
                {
                    label.spriteAsset = enabled ? SpriteHelper.LightControllerIcons : SpriteHelper.DarkControllerIcons;
                }
            }
        }

        private void ToggleDarkModeForSubmeshes(GameObject gameObject, bool hasSubmeshes, bool enabled)
        {
            if (!hasSubmeshes) return;

            var submeshes = gameObject.GetComponentsInChildren<TMP_SubMeshUI>(true);
            if (submeshes == null) return;

            foreach (var controllerButton in submeshes)
            {
                if (controllerButton?.spriteAsset == null) continue;
				if (controllerButton?.material == null) continue;

				controllerButton.material.color = enabled ? DarkModeText.normalColor : DefaultText.normalColor;
			}
        }

        private void SetBackgroundTexture(GameObject gameObject, bool darkModeEnabled)
        {
	        if (gameObject == null) return;

	        var textures = gameObject.GetComponentsInChildren<Image>().FirstOrDefault(x => x.name == "MenuPanelBackground");

	        if (textures != null)
	        {
		        if (SpriteHelper.OriginalBackground == null)
		        {
			        SpriteHelper.OriginalBackground = textures.sprite;
		        }

		        if (darkModeEnabled && SpriteHelper.DarkModeBackground != null)
		        {
			        textures.sprite = SpriteHelper.DarkModeBackground;
		        }
		        else
		        {
			        textures.sprite = SpriteHelper.OriginalBackground;
		        }
	        }
        }

        public static byte[] ExtractResource(string filename)
        {
	        Assembly a = Assembly.GetExecutingAssembly();
	        using (var resFilestream = a.GetManifestResourceStream(filename))
	        {
		        if (resFilestream == null) return new byte[0];
		        byte[] ba = new byte[resFilestream.Length];
		        resFilestream.Read(ba, 0, ba.Length);
		        return ba;
	        }
        }
	}
}
