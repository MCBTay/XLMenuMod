using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    public class LevelListItemPatch
    {
        [HarmonyPatch(typeof(LevelListItem), nameof(LevelListItem.Item), MethodType.Setter)]
        static class ItemPatch
        {
            static void Postfix(LevelListItem __instance, LevelInfo value)
            {

                if (__instance.LevelNameText.text.StartsWith("\\"))
                {
                    if (Main.Assets == null) return;
                    var sprites = Main.Assets.LoadAllAssets<TMP_SpriteAsset>().FirstOrDefault();

                    if (sprites == null) return;
                    __instance.LevelNameText.spriteAsset = sprites;

                    foreach (var sprite in sprites.spriteGlyphTable)
                    {
                        var x = sprite.sprite.textureRectOffset;                     
                    }

                    __instance.LevelNameText.SetText(__instance.LevelNameText.text.Replace("\\", "<sprite=10 tint=1 color=#FF0000FF>"));
                }
            }
        }
    }
}
