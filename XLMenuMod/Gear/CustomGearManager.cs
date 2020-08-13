using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using UnityModManagerNet;
using XLMenuMod.Interfaces;
using XLMenuMod.UserInterface;

namespace XLMenuMod.Gear
{
	public class CustomGearManager : CustomManager
	{
		private static CustomGearManager _instance;
		public static CustomGearManager Instance => _instance ?? (_instance = new CustomGearManager());

		public List<ICustomInfo> NestedOfficialItems { get; set; }
		private GearInfo[] LastLoaded { get; set; }

		public CustomGearManager()
		{
			NestedOfficialItems = new List<ICustomInfo>();
		}

		public void LoadNestedHairItems(object[] objectsToLoad = null)
		{
			NestedOfficialItems.Clear();

			var gearToLoad = (GearInfo[])objectsToLoad;
			if (gearToLoad == null) return;

			foreach (var gear in gearToLoad)
			{
				CustomFolderInfo parent = null;

				var nameSplit = gear.name.Split(' ');

				if (string.IsNullOrEmpty(nameSplit.FirstOrDefault())) continue;

				var hairStyle = nameSplit.FirstOrDefault();

				if (hairStyle.ToLower() == "long" || hairStyle.ToLower() == "short" || hairStyle.ToLower() == "curly") 
					hairStyle = $"{hairStyle} {nameSplit[1]}";

				AddFolder<CustomGearFolderInfo>(hairStyle, null, NestedOfficialItems, ref parent);
				AddItem(gear, parent.Children, ref parent);
			}

			NestedOfficialItems = SortList(NestedOfficialItems);
		}

		public void LoadNestedOfficialItems(object[] objectsToLoad = null)
		{
			NestedOfficialItems.Clear();

			var gearToLoad = (GearInfo[])objectsToLoad;
			if (gearToLoad == null) return;

			List<string> unbrandedItems = new List<string>
			{
				#region Decks
				"Tie Dye Light",
				"Skater XL Obstacles Deck",
				#endregion

				#region Grip
				"Black",
				"Camo Grey",
				"Easy Day Logo",
				#endregion

				#region Trucks
				"Black",
				"Silver",
				"White",
				#endregion

				#region Wheels
				"Purple",
				"Red/Blue Swirl",
				"White",
				#endregion
			};

			CustomFolderInfo parent = null;

			AddFolder<CustomGearFolderInfo>("Easy Day", null, NestedOfficialItems, ref parent);

			parent = null;

			var easyDayFolder = NestedOfficialItems.FirstOrDefault(x => x.GetName() == "\\Easy Day" && x.GetPath() == null) as CustomFolderInfo;

			foreach (var gear in gearToLoad)
			{
				parent = null;

				if (gear.name.StartsWith("Blank") || gear.name.StartsWith("Unbranded") || unbrandedItems.Contains(gear.name))
				{
					AddItem(gear, easyDayFolder.Children, ref easyDayFolder);
				}
				else
				{
					var nameSplit = gear.name.Split(' ');

					if (string.IsNullOrEmpty(nameSplit.FirstOrDefault())) continue;

					var brand = nameSplit.FirstOrDefault();

					if (brand == "Old" || brand == "New" || brand == "Santa" || brand == "Grimple") brand = $"{nameSplit[0]} {nameSplit[1]}";
					else if (brand == "The") brand = $"{nameSplit[0]} {nameSplit[1]} {nameSplit[2]}"; 
					else if (brand == "TWS") brand = "Transworld";

					AddFolder<CustomGearFolderInfo>(brand, null, NestedOfficialItems, ref parent);
					AddItem(gear, parent.Children, ref parent);
				}
			}

			NestedOfficialItems = SortList(NestedOfficialItems);
		}

		public override void LoadNestedItems(object[] objectsToLoad = null)
		{
			var gearToLoad = (GearInfo[]) objectsToLoad;
			if (gearToLoad == null) return;

			if (LastLoaded != null && LastLoaded == gearToLoad)
			{
				return;
			}

			LastLoaded = gearToLoad;
			NestedItems.Clear();

			foreach (var gear in gearToLoad)
			{
				GearInfo newGear = null;
				TextureChange textureChange = null;

				if (gear is GearInfoSingleMaterial singleMaterialGear)
				{
					textureChange = singleMaterialGear?.textureChanges?.FirstOrDefault();
					newGear = singleMaterialGear;
				}
				else if (gear is CharacterBodyInfo characterBodyInfo)
				{
					var materialChange = characterBodyInfo.materialChanges.FirstOrDefault();
					textureChange = materialChange?.textureChanges?.FirstOrDefault();
					newGear = characterBodyInfo;
				}

				if (textureChange == null) continue;
				if (string.IsNullOrEmpty(textureChange.texturePath) || !textureChange.texturePath.StartsWith(SaveManager.Instance.CustomGearDir)) continue;
				var texturePath = textureChange.texturePath;
				var textureSubPath = textureChange.texturePath.Replace(SaveManager.Instance.CustomGearDir + '\\', string.Empty);
				if (string.IsNullOrEmpty(textureSubPath)) continue;

				var folders = textureSubPath.Split('\\').ToList();
				if (!folders.Any()) continue;

				CustomFolderInfo parent = null;
				if (folders.Count == 1 || IsImage(folders.First()))
				{
					// This gear item is at the root.
					AddItem(newGear, NestedItems, ref parent);
					continue;
				}

				parent = null;
				string folderPath = SaveManager.Instance.CustomGearDir;

				foreach (var folder in folders)
				{
					if (IsImage(folder))
					{
						AddItem(newGear, parent == null ? NestedItems : parent.Children, ref parent);
					}
					else
					{
						folderPath = Path.Combine(folderPath, folder);
						AddFolder<CustomGearFolderInfo>(folder, folderPath, parent == null ? NestedItems : parent.Children, ref parent);
					}
				}
			}

			NestedItems = SortList(NestedItems);
		}

		private bool IsImage(string filepath)
		{
			string extension = Path.GetExtension(filepath).ToLower();

			if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".tga")
			{
				return true;
			}

			return false;
		}

		public override List<ICustomInfo> SortList(List<ICustomInfo> gear)
		{
			UserInterfaceHelper.Instance.SetSortLabelText(ref SortLabel, ((GearSortMethod)CurrentSort).ToString());

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
