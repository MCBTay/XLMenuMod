using System;
using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityModManagerNet;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
	public class LevelSelectionControllerPatch
    {
		[HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ConfigureHeaderView))]
		public static class ConfigureHeaderViewPatch
		{
			static void Postfix(LevelSelectionController __instance, MVCListHeaderView header)
			{
				//header.OnNextCategory += () =>
				//{
				//	CustomLevelManager.Instance.CurrentFolder = null;
				//	__instance.listView.UpdateList();
				//};
				//header.OnPreviousCategory += () =>
				//{
				//	CustomLevelManager.Instance.CurrentFolder = null;
				//	__instance.listView.UpdateList();
				//};

				CustomLevelManager.Instance.SortLabel.gameObject.SetActive(__instance.showCustom);

				if (CustomLevelManager.Instance.CurrentFolder != null && Main.BlackSprites != null)
				{
					header.Label.spriteAsset = Main.WhiteSprites;
					header.SetText(CustomLevelManager.Instance.CurrentFolder.GetName().Replace("\\", "<sprite=10> "));
				}
			}
		}

		[HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ConfigureListItemView))]
		public static class ConfigureListItemViewPatch
		{
			static void Postfix(ref MVCListItemView itemView)
			{
				if (itemView.Label.text.StartsWith("\\"))
				{
					if (Main.BlueSprites != null)
					{
						itemView.Label.spriteAsset = Main.WhiteSprites;

						int spriteIndex = itemView.Label.text.Equals("\\Easy Day") ? 8 : 10;
						itemView.Label.SetText(itemView.Label.text.Replace("\\", $"<sprite={spriteIndex} tint=1> "));
					}
				}
				else if (itemView.Label.text.Equals("..\\"))
				{
					if (Main.BlueSprites != null)
					{
						itemView.Label.spriteAsset = Main.WhiteSprites;
						itemView.Label.SetText(itemView.Label.text.Replace("..\\", "<sprite=9 tint=1> Go Back"));
					}
				}
			}
		}

		[HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.OnItemSelected))]
        public static class OnItemSelectedPatch
        {
            static bool Prefix(LevelSelectionController __instance, ref IndexPath index/*, ref LevelInfo level*/)
            {
	            if (CustomLevelManager.Instance.LastSelectedTime != 0d && Time.realtimeSinceStartup - CustomLevelManager.Instance.LastSelectedTime < 0.25f) return false;
	            CustomLevelManager.Instance.LastSelectedTime = Time.realtimeSinceStartup;

				var level = Traverse.Create(__instance).Method("GetLevelForIndex", index).GetValue<LevelInfo>();

				if (level is CustomLevelFolderInfo selectedFolder)
				{
					selectedFolder.FolderInfo.Children = CustomLevelManager.Instance.SortList(selectedFolder.FolderInfo.Children);

					var currentIndexPath = Traverse.Create(__instance.listView).Property<IndexPath>("currentIndexPath");

					if (selectedFolder.FolderInfo.GetName() == "..\\")
					{
						CustomLevelManager.Instance.CurrentFolder = selectedFolder.FolderInfo.Parent;
						currentIndexPath.Value = __instance.listView.currentIndexPath.Up();
					}
					else
					{
						CustomLevelManager.Instance.CurrentFolder = selectedFolder.FolderInfo;

						if (CustomLevelManager.Instance.CurrentFolder.Parent != null)
						{
							currentIndexPath.Value = __instance.listView.currentIndexPath.Sub(CustomLevelManager.Instance.CurrentFolder.Parent.Children.IndexOf(CustomLevelManager.Instance.CurrentFolder));
						}
						else
						{
							currentIndexPath.Value = __instance.listView.currentIndexPath.Sub(CustomLevelManager.Instance.NestedItems.IndexOf(CustomLevelManager.Instance.CurrentFolder));
						}
					}

					EventSystem.current.SetSelectedGameObject(null);
					__instance.listView.UpdateList();

					return false;
				}
				else if (level is CustomLevelInfo selectedLevel)
				{
					level = selectedLevel;
					return true;
				}
				else
				{
					CustomLevelManager.Instance.CurrentFolder = null;

					return true;
                }
            }
        }

        [HarmonyPatch(typeof(LevelSelectionController), "OnEnable")]
        public static class OnEnablePatch
        {
	        static void Postfix(LevelSelectionController __instance)
	        {
		        UpdateFontSize(__instance.listView.ItemPrefab.Label);

		        foreach (var item in __instance.listView.ItemViews)
		        {
					UpdateFontSize(item.Label);
		        }
	        }
			
	        private static void UpdateFontSize(TMP_Text label)
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
        }

        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.GetNumberOfItems))]
        public static class GetNumberOfItemsPatch
        {
	        static void Postfix(ref int __result, IndexPath index)
	        {
		        if (index[0] == 1)
		        {
			        if (CustomLevelManager.Instance.CurrentFolder != null &&
			            CustomLevelManager.Instance.CurrentFolder.Children != null &&
			            CustomLevelManager.Instance.CurrentFolder.Children.Any())
			        {
				        __result = CustomLevelManager.Instance.CurrentFolder.Children.Count;
			        }
			        else
			        {
						__result = CustomLevelManager.Instance.NestedItems.Count;
					}
		        }
	        }
        }

        [HarmonyPatch(typeof(LevelSelectionController), "GetLevelForIndex")]
        public static class GetLevelForIndexPatch
        {
	        static void Postfix(ref LevelInfo __result, IndexPath index)
	        {
		        if (index[0] == 1)
		        {
			        if (CustomLevelManager.Instance.CurrentFolder.HasChildren())
			        {
				        if (index.LastIndex < CustomLevelManager.Instance.CurrentFolder.Children.Count)
				        {
					        var customInfo = CustomLevelManager.Instance.CurrentFolder.Children.ElementAt(index.LastIndex);

					        if (customInfo.GetParentObject() is CustomLevelInfo)
						        __result = customInfo.GetParentObject() as CustomLevelInfo;
					        else if (customInfo.GetParentObject() is CustomLevelFolderInfo)
						        __result = customInfo.GetParentObject() as CustomLevelFolderInfo;
				        }
					}
			        else
			        {
						if (index.LastIndex < CustomLevelManager.Instance.NestedItems.Count)
						{
							var customInfo = CustomLevelManager.Instance.NestedItems.ElementAt(index.LastIndex);

							if (customInfo.GetParentObject() is CustomLevelInfo)
								__result = customInfo.GetParentObject() as CustomLevelInfo;
							else if (customInfo.GetParentObject() is CustomLevelFolderInfo)
								__result = customInfo.GetParentObject() as CustomLevelFolderInfo;
						}
					}
		        }
	        }
        }

        [HarmonyPatch(typeof(LevelSelectionController), "GetIndexForLevel")]
        public static class GetIndexForLevelPatch
		{
	        static void Postfix(ref IndexPath __result, LevelInfo level)
	        {
		        if (CustomLevelManager.Instance.CurrentFolder != null &&
		            CustomLevelManager.Instance.CurrentFolder.Children != null &&
		            CustomLevelManager.Instance.CurrentFolder.Children.Any())
		        {
			        if (level is CustomLevelInfo)
			        {
				        __result = new IndexPath(1, CustomLevelManager.Instance.CurrentFolder.Children.IndexOf((level as CustomLevelInfo).Info));
			        }
			        else if (level is CustomLevelFolderInfo)
			        {
				        __result = new IndexPath(1, CustomLevelManager.Instance.CurrentFolder.Children.IndexOf((level as CustomLevelFolderInfo).FolderInfo));
			        }
				}
		        else
		        {
					if (level is CustomLevelInfo)
					{
						__result = new IndexPath(1, CustomLevelManager.Instance.NestedItems.IndexOf((level as CustomLevelInfo).Info));
					}
					else if (level is CustomLevelFolderInfo)
					{
						__result = new IndexPath(1, CustomLevelManager.Instance.NestedItems.IndexOf((level as CustomLevelFolderInfo).FolderInfo));
					}
		        }
			}
		}

        [HarmonyPatch(typeof(LevelSelectionController), "Awake")]
        public static class AwakePatch
        {
            static void Postfix(LevelSelectionController __instance)
            {
                CustomLevelManager.Instance.SortLabel = UserInterfaceHelper.CreateSortLabel(__instance.listView.HeaderView.Label, __instance.listView.HeaderView.Label.transform, ((LevelSortMethod)CustomLevelManager.Instance.CurrentSort).ToString());
            }
        }
    }
}
