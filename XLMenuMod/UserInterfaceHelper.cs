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
            TMP_Text label;

            label = Instantiate(sourceText, parent);
            label.transform.localScale = new Vector3(1, 1, 1);

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
            label.SetText($"<voffset=0.25em><sprite={(int)GetYSpriteIndex()}></voffset> <size=60%><b>Sort By:</b> " + text.Replace('_', ' '));
        }

        public static ControllerIconSprite GetYSpriteIndex()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
                    if (str.Contains("Dual Shock") || str.Contains("DualShock"))
                    {
                        return ControllerIconSprite.PS4_Triangle_Button;
                    }
                    return ControllerIconSprite.XB1_Y;
                case RuntimePlatform.PS4:
                    return ControllerIconSprite.PS4_Triangle_Button;
                case RuntimePlatform.Switch:
                    return ControllerIconSprite.SWITCH_X;
                case RuntimePlatform.XboxOne:
                default:
                    return ControllerIconSprite.XB1_Y;
            }
        }
    }
}
