using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;
using XLMenuMod.UserInterface;

namespace XLMenuMod.Levels
{
	public class CustomLevelManager : CustomManager
    {
        private static CustomLevelManager _instance;
        public static CustomLevelManager Instance => _instance ?? (_instance = new CustomLevelManager());

        public override void LoadNestedItems(object[] objectsToLoad = null)
        {
            var levelsToRemove = new List<LevelInfo>();
            CustomFolderInfo parent = null;

            AddFolder<CustomLevelFolderInfo>("Easy Day", null, NestedItems, ref parent);

            parent = null;

            var easyDayFolder = NestedItems.FirstOrDefault(x => x.GetName() == "\\Easy Day" && x.GetPath() == null) as CustomFolderInfo;

            foreach (var level in LevelManager.Instance.CustomLevels)
            {
	            if (string.IsNullOrEmpty(level.path)) continue;

                if (Path.GetExtension(level.path).ToLower() == ".zip" || Path.GetExtension(level.path).ToLower() == ".rar") continue;

	            if (!level.isAssetBundle)
	            {
		            AddItem(level, easyDayFolder.Children, ref easyDayFolder);
		            continue;
	            }

	            var levelSubPath = level.path.Replace(SaveManager.Instance.CustomLevelsDir + '\\', string.Empty);

	            if (string.IsNullOrEmpty(levelSubPath)) continue;

	            var folders = levelSubPath.Split('\\').ToList();
	            if (folders == null || !folders.Any()) continue;

	            parent = null;
	            if (folders.Count == 1)
	            {
		            // This level is at the root
		            AddItem(LevelManager.Instance.LevelInfoForPath(level.path), NestedItems, ref parent);
		            continue;
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

            NestedItems = SortList(NestedItems);
        }

        public override List<ICustomInfo> SortList(List<ICustomInfo> levels)
        {
            UserInterfaceHelper.Instance.SetSortLabelText(ref SortLabel, ((LevelSortMethod)CurrentSort).ToString());

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
                case (int)LevelSortMethod.Name_DESC:
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
