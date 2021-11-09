using GameManagement;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Utilities;
using XLMenuMod.Utilities.Levels;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Patches.Level
{
	public static class LevelSelectionControllerPatch
	{
		[HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.ConfigureHeaderView))]
		public static class ConfigureHeaderViewPatch
		{
			static void Postfix(LevelSelectionController __instance, MVCListHeaderView header)
			{
				CustomLevelManager.Instance.SortLabel.gameObject.SetActive(__instance.showCustom);
				UserInterfaceHelper.Instance.UpdateLabelColor(CustomLevelManager.Instance.SortLabel, Main.Settings.EnableDarkMode ? UserInterfaceHelper.DarkModeText : UserInterfaceHelper.DefaultText);

				if (CustomLevelManager.Instance.CurrentFolder != null && SpriteHelper.MenuIcons != null)
				{
					var isEasyDay = CustomLevelManager.Instance.CurrentFolder.GetName().Equals("\\Easy Day");
					var isModIo = CustomLevelManager.Instance.CurrentFolder.GetName().Equals("\\mod.io");

					header.Label.spriteAsset = isEasyDay || isModIo ? SpriteHelper.BrandIcons : SpriteHelper.MenuIcons;
					header.SetText(CustomLevelManager.Instance.CurrentFolder.GetName().Replace("\\", $"<sprite name=\"{(isEasyDay || isModIo ? CustomLevelManager.Instance.CurrentFolder.GetName().Replace("\\", string.Empty).ToLower() : "folder_outline")}\" tint>"));
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
					if (SpriteHelper.MenuIcons != null)
					{
						var isEasyDay = itemView.Label.text.Equals("\\Easy Day");
						var isModIo = itemView.Label.text.Equals("\\mod.io");

						itemView.Label.spriteAsset = isEasyDay || isModIo ? SpriteHelper.BrandIcons : SpriteHelper.MenuIcons;
						itemView.SetText(itemView.Label.text.Replace("\\", $"<sprite name=\"{(isEasyDay || isModIo ? itemView.Label.text.Replace("\\", string.Empty).ToLower() : "folder_outline")}\" tint>"));
					}
				}
				else if (itemView.Label.text.Equals("..\\"))
				{
					if (SpriteHelper.MenuIcons != null)
					{
						itemView.Label.spriteAsset = SpriteHelper.MenuIcons;
						itemView.Label.SetText(itemView.Label.text.Replace("..\\", "<space=10px><sprite name=\"folder\" tint>Go Back"));
					}
				}
			}
		}

		[HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.OnItemSelected))]
        public static class OnItemSelectedPatch
        {
            static bool Prefix(LevelSelectionController __instance, IndexPath index)
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

						__instance.listView.UpdateList();
						__instance.listView.SetHighlighted(currentIndexPath.Value, true);
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

						__instance.listView.UpdateList();
						EventSystem.current.SetSelectedGameObject(null);
					}

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


        [HarmonyPatch(typeof(LevelSelectionController), nameof(LevelSelectionController.GetNumberOfItems))]
        public static class GetNumberOfItemsPatch
        {
	        static void Postfix(ref int __result, IndexPath index)
	        {
		        if (index[0] == 1)
		        {
			        if (CustomLevelManager.Instance.CurrentFolder.HasChildren())
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
				// Ensuring you're on the right game state such that multiplayer mod doesn't execute this code when it doesn't want to.
		        if (GameStateMachine.Instance.CurrentState.GetType() == typeof(LevelSelectionState) && index[0] == 1)
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
		        if (CustomLevelManager.Instance.CurrentFolder.HasChildren())
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
                CustomLevelManager.Instance.SortLabel = UserInterfaceHelper.Instance.CreateSortLabel(Main.Settings.EnableDarkMode, __instance.listView.HeaderView.Label, __instance.listView.HeaderView.transform, ((LevelSortMethod)CustomLevelManager.Instance.CurrentSort).ToString());
            }
        }
    }
}
