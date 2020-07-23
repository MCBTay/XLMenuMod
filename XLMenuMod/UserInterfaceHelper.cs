using System;
using Rewired;
using System.Linq;
using TMPro;
using UnityEngine;

namespace XLMenuMod
{
    public class UserInterfaceHelper : MonoBehaviour
    {
        public static TMP_Text CreateSortLabel(TMP_Text sourceText, Transform parent, string sort)
        {
            TMP_Text label = Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);
            label.color = Color.black;
            
            var controllerIcons = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>().FirstOrDefault(x => x.name == "ControllerIcons");
            if (controllerIcons != null)
            {
                label.spriteAsset = controllerIcons;
            }

            label.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.gameObject.SetActive(false);

            SetSortLabelText(ref label, sort.Replace('_', ' '));

            label.transform.Translate(new Vector3(0, -30, 0));

            return label;
        }

        public static void SetSortLabelText(ref TMP_Text label, string text)
        {
            var sortLabelText = $"<voffset=0.25em><sprite={GetSpriteIndex_YButton()}></voffset> <size=60%><b>Sort By:</b> " + text.Replace('_', ' ');
            //var defaultLabelText = $"<size=60%><voffset=0.25em><sprite={GetSpriteIndex_XButton()}></voffset> <b>Set Default</b>";

            label.SetText(sortLabelText); //+ defaultLabelText);
        }

        public static int GetSpriteIndex_YButton()
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

        public static int GetSpriteIndex_XButton()
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

        public static void SetCategoryButtonLabel(ref TMP_Text label, string text, string defaultText, bool useDefault = true)
        {
            if (useDefault)
            {
                label.SetText(defaultText);
            }
            else
            {
                label.spriteAsset = Main.BlackSprites;
                label.SetText(text.Replace("\\", "<sprite=10> "));
            }
        }
    }
}
