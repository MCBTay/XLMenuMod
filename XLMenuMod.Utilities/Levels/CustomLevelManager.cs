using ModIO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Utilities.Extensions;
using XLMenuMod.Utilities.Interfaces;
using XLMenuMod.Utilities.UserInterface;

namespace XLMenuMod.Utilities.Levels
{
	public class CustomLevelManager : CustomManager
    {
        private static CustomLevelManager _instance;
        public static CustomLevelManager Instance => _instance ?? (_instance = new CustomLevelManager());

        public override void LoadNestedItems(object[] objectsToLoad = null)
        {
            CustomFolderInfo parent = null;

            AddFolder<CustomLevelFolderInfo>("Easy Day", null, NestedItems, ref parent);
            CustomFolderInfo easyDayFolder = NestedItems.FirstOrDefault(x => x.GetName() == "\\Easy Day" && x.GetPath() == null) as CustomFolderInfo;

            parent = null;

            CustomFolderInfo modIoFolder = null;
            var modIoMaps = LevelManager.Instance.ModLevels.Any(x => x.path.IsSubPathOf(PluginSettings.INSTALLATION_DIRECTORY));
            if (modIoMaps)
            {
	            AddFolder<CustomLevelFolderInfo>("mod.io", null, NestedItems, ref parent);
	            modIoFolder = NestedItems.FirstOrDefault(x => x.GetName() == "\\mod.io" && x.GetPath() == null) as CustomFolderInfo;
            }

            foreach (var level in LevelManager.Instance.ModLevels)
            {
	            AddLevel(level, easyDayFolder, modIoFolder, ref parent);
            }

            parent = null;
            foreach (var level in LevelManager.Instance.CommunityLevels)
            {
                AddLevel(level, easyDayFolder, modIoFolder, ref parent);
            }

            NestedItems = SortList(NestedItems);
        }

        private void AddLevel(LevelInfo level, CustomFolderInfo easyDayFolder, CustomFolderInfo modIoFolder, ref CustomFolderInfo parent)
        {
	        if (string.IsNullOrEmpty(level.path)) return;

	        var extension = Path.GetExtension(level.path).ToLower();
	        if (extension == ".zip" || extension == ".rar" || extension == ".json") return;

	        if (!level.isAssetBundle)
	        {
		        AddItem(level, easyDayFolder.Children, ref easyDayFolder);
		        return;
	        }
            
	        if (level.path.IsSubPathOf(PluginSettings.INSTALLATION_DIRECTORY))
	        {
                AddItem(level, modIoFolder.Children, ref modIoFolder);
                return;
	        }

	        string levelSubPath = level.path.Replace(SaveManager.Instance.CustomLevelsDir + '\\', string.Empty);
	        if (string.IsNullOrEmpty(levelSubPath)) return;

            var folders = levelSubPath.Split('\\').ToList();
	        if (!folders.Any()) return;

	        parent = null;
	        if (folders.Count == 1)
	        {
		        // This level is at the root
		        AddItem(LevelManager.Instance.LevelInfoForPath(level.path), NestedItems, ref parent);
		        return;
	        }

	        parent = null;
	        var folderPath = SaveManager.Instance.CustomLevelsDir;

	        for (int i = 0; i < folders.Count; i++)
	        {
		        var folder = folders.ElementAt(i);
		        if (folder == null) continue;

		        if (folder == folders.Last())
		        {
			        AddItem(LevelManager.Instance.LevelInfoForPath(level.path), parent == null ? NestedItems : parent.Children, ref parent);
		        }
		        else
		        {
			        folderPath = Path.Combine(folderPath, folder);
			        AddFolder<CustomLevelFolderInfo>(folder, folderPath, parent == null ? NestedItems : parent.Children, ref parent);
		        }
	        }
        }

        public override List<ICustomInfo> SortList(List<ICustomInfo> levels)
        {
            UserInterfaceHelper.Instance.SetSortLabelText(ref _sortLabel, ((LevelSortMethod)CurrentSort).ToString());

            List<ICustomInfo> sorted;

            switch (CurrentSort)
            {
                //case (int)LevelSortMethod.Recently_Played:
                //    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(y => y.GetLastUsage()).ToList();
                //    break;
                //case (int)LevelSortMethod.Least_Played:
                //    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetUsageCount()).ToList();
                //    break;
                //case (int)LevelSortMethod.Most_Played:
                //    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetUsageCount()).ToList();
                //    break;
                case (int)LevelSortMethod.Newest:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetModifiedDate(false)).ToList();
                    break;
                case (int)LevelSortMethod.Oldest:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetModifiedDate(true)).ToList();
                    break;
                case (int)LevelSortMethod.Filesize_ASC:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethod.Filesize_DESC:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethod.Name_ASC:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetName()).ToList();
                    break;
                case (int)LevelSortMethod.Author_ASC:
	                sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => string.IsNullOrEmpty(((LevelInfo)x.GetParentObject()).author)).ThenBy(x => ((LevelInfo)x.GetParentObject()).author).ToList();
	                break;
                case (int)LevelSortMethod.Author_DESC:
	                sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => string.IsNullOrEmpty(((LevelInfo)x.GetParentObject()).author)).ThenByDescending(x => ((LevelInfo)x.GetParentObject()).author).ToList();
                    break;
                default:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetName()).ToList();
                    break;
            }

            return sorted;
        }           

        // Not currently used, but already written and may be useful later.
        public override void OnPreviousSort<T>()
        {
            base.OnPreviousSort<LevelSortMethod>();

            EventSystem.current.SetSelectedGameObject(null);
            Object.FindObjectOfType<LevelSelectionController>()?.listView?.UpdateList();
        }

        public override void OnNextSort<T>()
        {
            base.OnNextSort<LevelSortMethod>();

            EventSystem.current.SetSelectedGameObject(null);
            Object.FindObjectOfType<LevelSelectionController>()?.listView?.UpdateList();
        }
    }
}
