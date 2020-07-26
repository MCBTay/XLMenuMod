using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear
{
	public class CustomGearManager : CustomManager
	{
		private static CustomGearManager _instance;
		public static CustomGearManager Instance => _instance ?? (_instance = new CustomGearManager());

		public static int CurrentGearFilterIndex { get; set; }

		public override void LoadNestedItems(object[] objectsToLoad = null)
		{
			NestedItems.Clear();

			var gearToLoad = (GearInfo[]) objectsToLoad;
			if (gearToLoad == null) return;
			
			foreach (var gear in gearToLoad)
			{
				var singleMaterialGear = gear as GearInfoSingleMaterial;

				// For now all I saw was one texture change per gear type, so assuming first.
				var textureChange = singleMaterialGear?.textureChanges?.FirstOrDefault();
				if (textureChange == null) continue;

				if (string.IsNullOrEmpty(textureChange.texturePath) || !textureChange.texturePath.StartsWith(SaveManager.Instance.CustomGearDir)) continue;

				var textureSubPath = textureChange.texturePath.Replace(SaveManager.Instance.CustomGearDir + '\\', string.Empty);

				if (string.IsNullOrEmpty(textureSubPath)) continue;

				var folders = textureSubPath.Split('\\').ToList();
				if (!folders.Any()) continue;

				CustomFolderInfo parent = null;
				if (folders.Count == 1 || Path.GetExtension(folders.First()).ToLower() == ".png")
				{
					// This gear item is at the root.
					AddItem(singleMaterialGear, ref parent);
					continue;
				}

				parent = null;
				foreach (var folder in folders)
				{
					if (Path.GetExtension(folder).ToLower() == ".png")
					{
						AddItem(singleMaterialGear, ref parent);
					}
					else
					{
						AddFolder<CustomGearFolderInfo>(folder, Path.GetDirectoryName(textureChange.texturePath), ref parent);
					}
				}
			}

			NestedItems = SortList(NestedItems);
		}

		public override List<ICustomInfo> SortList(List<ICustomInfo> gear)
		{
			UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((GearSortMethod)CurrentSort).ToString());

			List<ICustomInfo> sorted;

			switch (CurrentSort)
			{
				case (int)GearSortMethod.Newest:
					sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetModifiedDate(false)).ToList();
					break;
				case (int)GearSortMethod.Oldest:
					sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetModifiedDate(true)).ToList();
					break;
				case (int)GearSortMethod.Name_ASC:
					sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetName()).ToList();
					break;
				case (int)GearSortMethod.Name_DESC:
				default:
					sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetName()).ToList();
					break;
			}

			return sorted;
		}

		public override void OnPreviousSort<T>()
		{
			base.OnPreviousSort<GearSortMethod>();

			EventSystem.current.SetSelectedGameObject(null);
			GearSelectionController.Instance.listView.UpdateList();
		}

		public override void OnNextSort<T>()
		{
			base.OnNextSort<GearSortMethod>();

			EventSystem.current.SetSelectedGameObject(null);
			GearSelectionController.Instance.listView.UpdateList();
		}
	}
}
