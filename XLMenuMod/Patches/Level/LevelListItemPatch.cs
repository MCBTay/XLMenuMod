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
                    if (Main.BlueSprites != null)
                    {
                        __instance.LevelNameText.spriteAsset = Main.BlueSprites;
                        __instance.LevelNameText.SetText(__instance.LevelNameText.text.Replace("\\", "<sprite=10 tint=1> "));
                    }
                    
                }
                else if (__instance.LevelNameText.text.Equals("..\\"))
                {
                    if (Main.BlueSprites != null)
                    {
                        __instance.LevelNameText.spriteAsset = Main.BlueSprites;
                        __instance.LevelNameText.SetText(__instance.LevelNameText.text.Replace("..\\", "<sprite=9 tint=1> Go Back"));
                    }
                }
            }
        }
    }
}
