using Rewired;
using System.Linq;
using TMPro;
using UnityEngine;

namespace XLMenuMod
{
    public class UserInterfaceHelper : MonoBehaviour
    {
        public static void CreateSortLabel(ref TMP_Text newLabel, TMP_Text sourceText, Transform parent, string sort)
        {
            if (newLabel != null) return;

            newLabel = Instantiate(sourceText, parent);
            newLabel.transform.localScale = new Vector3(1, 1, 1);

            var controllerIcons = Resources.FindObjectsOfTypeAll<TMP_SpriteAsset>().FirstOrDefault(x => x.name == "ControllerIcons");
            if (controllerIcons != null)
            {
                newLabel.spriteAsset = controllerIcons;
            }

            newLabel.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
            newLabel.gameObject.SetActive(false);

            newLabel.SetText($"<voffset=0.25em><sprite={(int)GetYSpriteIndex()}></voffset> <size=60%><b>Sort By:</b> " + sort.Replace('_', ' '));
            newLabel.transform.Translate(new Vector3(0, -30, 0));
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
