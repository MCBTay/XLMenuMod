using HarmonyLib;
using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine.EventSystems;
using XLMenuMod.Gear;

namespace XLMenuMod.Patches.Gear
{
	public class GearSelectionControllerPatch
	{
		[HarmonyPatch(typeof(GearSelectionController), nameof(GearSelectionController.ConfigureHeaderView))]
		public static class ConfigureHeaderViewPatch
		{
			static void Postfix(GearSelectionController __instance, MVCListHeaderView itemView)
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

				CustomGearManager.Instance.SortLabel.gameObject.SetActive(__instance.listView.currentIndexPath[1] >= 12);

				if (CustomGearManager.Instance.CurrentFolder != null && Main.WhiteSprites != null)
				{
					itemView.Label.spriteAsset = Main.WhiteSprites;
					itemView.SetText(CustomGearManager.Instance.CurrentFolder.GetName().Replace("\\", "<sprite=10> "));
				}
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), nameof(GearSelectionController.ConfigureListItemView))]
		public static class ConfigureListItemViewPatch
		{
			static void Postfix(IndexPath index, ref MVCListItemView itemView)
			{
				if (Main.WhiteSprites != null)
				{
					itemView.Label.spriteAsset = Main.WhiteSprites;
				}
				itemView.Label.richText = true;

				switch (Main.Settings.FontSize)
				{
					case FontSizePreset.Small:
						itemView.Label.fontSize = 30;
						break;
					case FontSizePreset.Smaller:
						itemView.Label.fontSize = 24;
						break;
					case FontSizePreset.Normal:
					default:
						itemView.Label.fontSize = 36;
						break;
				}

				if (index.depth >= 3)
				{
					GearInfo gearAtIndex = GearDatabase.Instance.GetGearAtIndex(index, out bool _);

					if (gearAtIndex == (GearInfo)null)
					{
						itemView.SetText("NOT FOUND", false);
						Traverse.Create(GearSelectionController.Instance).Method("SetIsEquippedIndicators", itemView, false).GetValue();
					}
					else
					{
						if (gearAtIndex.name.StartsWith("\\"))
						{
							itemView.SetText(gearAtIndex.name.Replace("\\", "<space=15px><sprite=10 tint=1>"), true);
						}
						else if (gearAtIndex.name.Equals("..\\"))
						{
							itemView.SetText(gearAtIndex.name.Replace("..\\", "<space=15px><sprite=9 tint=1>Go Back"), true);
						}
						else
						{
							itemView.SetText(gearAtIndex.name, true);
						}
						Traverse.Create(GearSelectionController.Instance).Method("SetIsEquippedIndicators", itemView, GearSelectionController.Instance.previewCustomizer.HasEquipped(gearAtIndex)).GetValue();
					}
				}
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), nameof(GearSelectionController.GetNumberOfItems))]
		public static class GetNumberOfItemsPatch
		{
			static void Postfix(ref int __result, IndexPath index)
			{
				if (index.depth >= 3)
				{
					__result = CustomGearManager.Instance.CurrentFolder.HasChildren() ? CustomGearManager.Instance.CurrentFolder.Children.Count : CustomGearManager.Instance.NestedItems.Count;
				}
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), "ListView_OnItemSelectedEvent")]
		public static class ListView_OnItemSelectedEventPatch
		{
			static bool Prefix(GearSelectionController __instance, IndexPath index)
			{
				var gear = GearDatabase.Instance.GetGearAtIndex(index);

				if (gear is CustomGearFolderInfo selectedFolder)
				{
					selectedFolder.FolderInfo.Children = CustomGearManager.Instance.SortList(selectedFolder.FolderInfo.Children);

					var currentIndexPath = Traverse.Create(__instance.listView).Property<IndexPath>("currentIndexPath");

					if (selectedFolder.FolderInfo.GetName() == "..\\")
					{
						CustomGearManager.Instance.CurrentFolder = selectedFolder.FolderInfo.Parent;
						currentIndexPath.Value = __instance.listView.currentIndexPath.Up();
					}
					else
					{
						CustomGearManager.Instance.CurrentFolder = selectedFolder.FolderInfo;

						if (CustomGearManager.Instance.CurrentFolder.Parent != null)
						{
							currentIndexPath.Value = __instance.listView.currentIndexPath.Sub(CustomGearManager.Instance.CurrentFolder.Parent.Children.IndexOf(CustomGearManager.Instance.CurrentFolder));
						}
						else
						{
							currentIndexPath.Value = __instance.listView.currentIndexPath.Sub(CustomGearManager.Instance.NestedItems.IndexOf(CustomGearManager.Instance.CurrentFolder));
						}
					}

					EventSystem.current.SetSelectedGameObject(null);
					__instance.listView.UpdateList();

					return false;
				}

				if (index.depth >= 3)
				{
					if (__instance.previewCustomizer.HasEquipped((ICharacterCustomizationItem)gear))
						return false;
					try
					{
						__instance.previewCustomizer.EquipGear(gear);
						__instance.previewCustomizer.OnlyShowEquippedGear();
						Traverse.Create(__instance).Field<bool>("didChangeGear").Value = true;
					}
					catch (Exception ex)
					{
					}
					__instance.Save();
					__instance.listView.UpdateList();

					return false;
				}

				CustomGearManager.Instance.CurrentFolder = null;
				return true;
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), "ListView_OnItemHighlightedEvent")]
		public static class ListView_OnItemHighlightedEventPatch
		{
			/// <summary>
			/// Most of this code comes directly from the default GearSelectionController, we just allow you do exceed an index.depth of 3.
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="index"></param>
			static void Postfix(GearSelectionController __instance, IndexPath index)
			{
				if (index.depth >= 3)
				{
					GearInfo gearAtIndex1 = GearDatabase.Instance.GetGearAtIndex(index);
					if (gearAtIndex1 == (GearInfo)null)
						return;
					List<GearInfo> toBeCachedGear = new List<GearInfo>();
					for (int steps = -__instance.preloadedItemsPerSide; steps <= __instance.preloadedItemsPerSide; ++steps)
					{
						GearInfo gearAtIndex2 = GearDatabase.Instance.GetGearAtIndex(index.Horizontal(steps));
						if (gearAtIndex2 != (GearInfo)null)
							toBeCachedGear.Add(gearAtIndex2);
					}

					if (gearAtIndex1 is CustomGearFolderInfo)
					{
						__instance.previewCustomizer.PreviewItem(null, toBeCachedGear);
					}
					else
					{
						__instance.previewCustomizer.PreviewItem(gearAtIndex1, toBeCachedGear);
					}
				}
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), "Awake")]
		public static class AwakePatch
		{
			static void Postfix(GearSelectionController __instance)
			{
				CustomGearManager.Instance.SortLabel = UserInterfaceHelper.CreateSortLabel(__instance.listView.HeaderView.Label, __instance.listView.HeaderView.Label.transform, ((GearSortMethod)CustomGearManager.Instance.CurrentSort).ToString());
			}
		}

		[HarmonyPatch(typeof(GearSelectionController), "Update")]
		public static class UpdatePatch
		{
			static bool Prefix(GearSelectionController __instance)
			{
				var player = Traverse.Create(__instance).Field("player").GetValue<Player>();
				if (player.GetButtonDown("Y"))
				{
					UISounds.Instance?.PlayOneShotSelectionChange();

					CustomGearManager.Instance.OnNextSort<GearSortMethod>();
					return false;
				}

				if (__instance.listView.currentIndexPath.depth >= 3)
				{
					if (CustomGearManager.Instance.CurrentFolder == null) return true;
					if (!PlayerController.Instance.inputController.player.GetButtonDown("B")) return true;

					if (!Main.Settings.DisableBToMoveUpDirectory)
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						CustomGearManager.Instance.CurrentFolder = CustomGearManager.Instance.CurrentFolder.Parent;
						GearSelectionController.Instance.listView.UpdateList(__instance.listView.currentIndexPath.Up());
						return false;
					}
				}

				return true;
			}
		}
	}
}
