using ModIO;
using SkaterXL.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using XLMenuMod.Utilities.Extensions;
using XLMenuMod.Utilities.Interfaces;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Utilities.Gear
{
	public class CustomGearManager : CustomManager
	{
		private static CustomGearManager _instance;
		public static CustomGearManager Instance => _instance ?? (_instance = new CustomGearManager());

		public List<ICustomInfo> NestedOfficialItems { get; set; }
		private GearInfo[] LastLoaded { get; set; }

		public IList<KeyValuePair<string, string>> InstalledGearMods;

		public CustomGearManager()
		{
			NestedOfficialItems = new List<ICustomInfo>();
			InstalledGearMods = new List<KeyValuePair<string, string>>();
		}

		public void LoadNestedHairItems(GearInfo[] objectsToLoad = null)
		{
			NestedOfficialItems.Clear();

			if (objectsToLoad == null) return;

			foreach (var gear in objectsToLoad)
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

		public void LoadNestedOfficialItems(GearInfo[] gearToLoad = null)
		{
			NestedOfficialItems.Clear();

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

			var easyDayItemsExist = gearToLoad.Any(x =>
				x.name.StartsWith("Blank") ||
				x.name.StartsWith("Unbranded") ||
				x.name.StartsWith("Denim") ||
				unbrandedItems.Contains(x.name));

			if (easyDayItemsExist)
				AddFolder<CustomGearFolderInfo>("Easy Day", null, NestedOfficialItems, ref parent);

			parent = null;

			var easyDayFolder = NestedOfficialItems.FirstOrDefault(x => x.GetName() == "\\Easy Day" && x.GetPath() == null) as CustomFolderInfo;

			foreach (var gear in gearToLoad)
			{
				parent = null;

				if (gear.name.StartsWith("Blank") || gear.name.StartsWith("Unbranded") || gear.name.StartsWith("Denim") || unbrandedItems.Contains(gear.name))
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

			CustomFolderInfo modIoFolder = null;

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
				
				if (textureChange == null || string.IsNullOrEmpty(textureChange.texturePath)) continue;

				var isGearFolder = textureChange.texturePath.IsSubPathOf(SaveManager.Instance.CustomGearDir);
				var isModIo = textureChange.texturePath.IsSubPathOf(PluginSettings.INSTALLATION_DIRECTORY);
				
				CustomFolderInfo parent = null;
				if (isModIo)
				{
					AddFolder<CustomGearFolderInfo>("mod.io", null, NestedItems, ref parent);
					modIoFolder = NestedItems.FirstOrDefault(x => x.GetName() == "\\mod.io" && x.GetPath() == null) as CustomFolderInfo;

					var mod = InstalledGearMods.FirstOrDefault(x => textureChange.texturePath.IsSubPathOf(x.Key));

					AddFolder<CustomGearFolderInfo>(mod.Value, null, modIoFolder.Children, ref parent);
					AddItem(newGear, parent.Children, ref modIoFolder);

					continue;
				}
				 
				if (!isGearFolder && !isModIo) continue;

				string textureSubPath = string.Empty;
				string folderPath = string.Empty;

				if (isGearFolder)
				{
					textureSubPath = textureChange.texturePath.Replace(SaveManager.Instance.CustomGearDir + '\\', string.Empty);
					folderPath = SaveManager.Instance.CustomGearDir;
				}
				else if (isModIo) 
				{
					textureSubPath = textureChange.texturePath.Replace(PluginSettings.INSTALLATION_DIRECTORY + '\\', string.Empty);
					folderPath = PluginSettings.INSTALLATION_DIRECTORY;
				}
				
				if (string.IsNullOrEmpty(textureSubPath) || string.IsNullOrEmpty(folderPath)) continue;

				var folders = textureSubPath.Split('\\').ToList();
				if (!folders.Any()) continue;

				parent = null;
				if (folders.Count == 1 || IsImage(folders.First()))
				{
					// This gear item is at the root.
					AddItem(newGear, NestedItems, ref parent);
					continue;
				}

				parent = null;

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
