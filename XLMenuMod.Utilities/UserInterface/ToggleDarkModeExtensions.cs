using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLMenuMod.Utilities.UserInterface
{
	public static class ToggleDarkModeExtensions
	{
		public static void ToggleDarkMode(this GameObject gameObject, bool enabled, bool hasStaticText = false, bool hasSubmeshes = false)
		{
			if (gameObject == null) return;

			if (hasStaticText)
			{
				gameObject.GetComponentsInChildren<TMP_Text>(true).ToggleDarkMode(enabled);
			}

			if (hasSubmeshes)
			{
				gameObject.GetComponentsInChildren<TMP_SubMeshUI>(true).ToggleDarkMode(enabled);
			}

			gameObject.GetComponentsInChildren<MenuButton>(true).ToggleDarkMode(enabled);
			gameObject.GetComponentsInChildren<MenuSlider>(true).ToggleDarkMode(enabled);
			gameObject.GetComponentsInChildren<MenuToggle>(true).ToggleDarkMode(enabled);

			gameObject.GetComponentInChildren<ChallengeSummaryController>(true).ToggleDarkMode(enabled);
			gameObject.GetComponentInChildren<ChallengeTrickListController>(true).ToggleDarkMode(enabled);
			gameObject.GetComponentInChildren<MVCListView>(true).ToggleDarkMode(enabled);

			// This is for the replay editor UIs and menu background
			gameObject.GetComponentsInChildren<Image>(true).ToggleDarkMode(enabled);
		}

		public static void ToggleDarkMode(this MenuButton[] buttons, bool enabled)
		{
			if (buttons == null || !buttons.Any()) return;

			foreach (var button in buttons)
			{
				button.ToggleDarkMode(enabled);
			}
		}

		public static void ToggleDarkMode(this MenuSlider[] sliders, bool enabled)
		{
			if (sliders == null || !sliders.Any()) return;

			foreach (var slider in sliders)
			{
				slider.ToggleDarkMode(enabled);
			}
		}

		public static void ToggleDarkMode(this MenuToggle[] toggles, bool enabled)
		{
			if (toggles == null || !toggles.Any()) return;

			foreach (var toggle in toggles)
			{
				toggle.ToggleDarkMode(enabled);
			}
		}

		public static void ToggleDarkMode(this Image[] images, bool enabled)
		{
			if (images == null || !images.Any()) return;

			foreach (var image in images)
            {
                image.UpdateMenuPanelBackgroundSprite(enabled);
				image.UpdateReplayEditorBackgroundSprite(enabled);
                image.UpdateReplayEditorHeaderBackgroundSprite(enabled);
            }
		}

		/// <summary>
		/// For the menu background sprite
		/// </summary>
        private static void UpdateMenuPanelBackgroundSprite(this Image image, bool enabled)
        {
            if (image.name != "MenuPanelBackground") return;

            SpriteHelper.OriginalBackground ??= image.sprite;

            image.sprite = enabled && SpriteHelper.DarkModeBackground != null ? SpriteHelper.DarkModeBackground : SpriteHelper.OriginalBackground;
        }

        /// <summary>
        /// For the replay editor background sprite
        /// </summary>
		private static void UpdateReplayEditorBackgroundSprite(this Image image, bool enabled)
        {
            if (image.mainTexture.name != "PanelTransparent") return;

            UserInterfaceHelper.OriginalReplayBackground ??= image.mainTexture as Texture2D;

            var texture = enabled ? SpriteHelper.DarkModeReplayBackground : UserInterfaceHelper.OriginalReplayBackground;
            if (texture == null) return;

            var rectangle = new Rect(0, 0, texture.width, texture.height);
            var pivot = new Vector2(72f, 72f);
            var border = new Vector4(30f, 34f, 29f, 27f);

            image.sprite = Sprite.Create(texture, rectangle, pivot, 300, 0, SpriteMeshType.Tight, border);
		}

		/// <summary>
		/// For the replay editor "header" background
		/// </summary>
		private static void UpdateReplayEditorHeaderBackgroundSprite(this Image image, bool enabled)
        {
            image.color = enabled ? UserInterfaceHelper.DarkModeReplayHeaderColor : UserInterfaceHelper.OriginalReplayHeaderColor;
		}


        public static void ToggleDarkMode(this Selectable button, bool enabled)
		{
			if (button == null) return;

			ColorBlock color;

			if (button is MenuSlider menuSlider)
			{
				if (menuSlider.selectionIndicator == null)
				{
					color = enabled ? UserInterfaceHelper.DarkModeSliderText : UserInterfaceHelper.DefaultSliderText;
				}
				else
				{
					color = enabled ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
				}
			}
			else
			{
				color = enabled ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
			}

			button.colors = color;
		}

		public static void ToggleDarkMode(this TMP_Text[] labels, bool enabled)
		{
			if (labels == null || !labels.Any()) return;

			foreach (var label in labels)
			{
				label.ToggleDarkMode(enabled);
			}
		}

		public static void ToggleDarkMode(this TMP_Text label, bool enabled)
		{
			if (label == null) return;

			// TODO: Compare this with the other blue I have on file.  Perhaps it's the same?
			var blue = new Color(0.007843138f, 0.4509804f, 1.000f, 1.000f);

			if (Mathf.Approximately(label.color.r, blue.r) &&
				Mathf.Approximately(label.color.g, blue.g) &&
				Mathf.Approximately(label.color.b, blue.b) &&
				Mathf.Approximately(label.color.a, blue.a)) return;

			var color = enabled ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
			label.color = color.normalColor;

			if (label.text.Contains("<sprite") && label.spriteAsset.name.Contains("Controller"))
			{
				label.spriteAsset = enabled ? SpriteHelper.LightControllerIcons : SpriteHelper.DarkControllerIcons;
			}
		}

		public static void ToggleDarkMode(this TMP_SubMeshUI[] submeshes, bool enabled)
		{
			if (submeshes == null || !submeshes.Any()) return;

			foreach (var submesh in submeshes)
			{
				if (submesh?.spriteAsset != null && submesh?.material != null)
				{
					submesh.material.color = enabled ? UserInterfaceHelper.DarkModeText.normalColor : UserInterfaceHelper.DefaultText.normalColor;
				}
			}
		}

		#region MVCListViews
		public static void ToggleDarkMode(this MVCListView listView, bool enabled)
		{
			if (listView == null) return;

			//TODO: Should this even be here? It's pretty irrelvant to dark mode, although it does ensure the font size gets set more often?
			UserInterfaceHelper.Instance.UpdateFontSize(listView.ItemPrefab.Label);

			listView.ItemPrefab.ToggleDarkMode(enabled);
			listView.HeaderView.ToggleDarkMode(enabled);

			foreach (var item in listView.ItemViews)
			{
				item.ToggleDarkMode(enabled);
			}
		}

		public static void ToggleDarkMode(this MVCListItemView listItemView, bool enabled)
		{
			if (listItemView == null) return;

			//TODO: Should this even be here? It's pretty irrelvant to dark mode, although it does ensure the font size gets set more often?
			UserInterfaceHelper.Instance.UpdateFontSize(listItemView.Label);

			listItemView.Label.ToggleDarkMode(enabled);
		}

		public static void ToggleDarkMode(this MVCListHeaderView header, bool enabled)
		{
			if (header == null || header.Label == null) return;

			var textColor = enabled ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText;
			header.Label.color = textColor.normalColor;
		}
		#endregion

		#region Challenges
		public static void ToggleDarkMode(this ChallengeSummaryController controller, bool enabled)
		{
			if (controller == null) return;

			var challengeFailedText = controller.FailureHeader.GetComponentInChildren<TMP_Text>();
			if (challengeFailedText == null) return;

			challengeFailedText.ToggleDarkMode(enabled);
		}

		public static void ToggleDarkMode(this ChallengeTrickListController controller, bool enabled)
		{
			if (controller == null) return;

			var itemViews = Traverse.Create(controller).Field<List<ChallengeTrickItemView>>("ItemViews").Value;
			if (itemViews != null && itemViews.Any())
			{
				itemViews.ToggleDarkMode(enabled);
			}
		}
		#endregion
	}
}
