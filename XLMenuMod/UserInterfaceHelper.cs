using System;
using Rewired;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityModManagerNet;

namespace XLMenuMod
{
    public class UserInterfaceHelper : MonoBehaviour
    {
        public static TMP_Text CreateSortLabel(TMP_Text sourceText, Transform parent, string sort, int yOffset = -50)
        {
            TMP_Text label = Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);
            label.color = Color.black;

            var controllerIcons = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>().FirstOrDefault(x => x.name == "ControllerIcons_ReversedOut_Greyish");
            if (controllerIcons != null)
            {
                label.spriteAsset = controllerIcons;

                foreach (var icon in controllerIcons.spriteCharacterTable)
                {
	                UnityModManager.Logger.Log(icon.name);
                }
            }

            label.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            label.gameObject.SetActive(false);

            SetSortLabelText(ref label, sort.Replace('_', ' '));

            label.transform.localPosition = Vector3.zero;
            label.transform.Translate(new Vector3(0, yOffset, 0));

            return label;
        }

        public static void SetSortLabelText(ref TMP_Text label, string text)
        {
            var sortLabelText = $"<size=80%><sprite={GetSpriteIndex_YButton_Gray()}> <size=60%><b>Sort By:</b> " + text.Replace('_', ' ');
            //var defaultLabelText = $"<size=60%><voffset=0.25em><sprite={GetSpriteIndex_XButton()}></voffset> <b>Set Default</b>";

            label?.SetText(sortLabelText); //+ defaultLabelText);
        }

        public static int GetSpriteIndex_YButton_Gray()
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
    }
}
