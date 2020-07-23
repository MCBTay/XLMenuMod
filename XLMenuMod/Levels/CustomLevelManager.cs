using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelManager : CustomManager
    {
        private static CustomLevelManager _instance;
        public static CustomLevelManager Instance => _instance ?? (_instance = new CustomLevelManager());

        public List<string> LoadNestedLevelPaths(string directoryToSearch = null)
        {
            var nestedLevels = new List<string>();

            if (directoryToSearch == null) directoryToSearch = SaveManager.Instance.CustomLevelsDir;

            if (Directory.Exists(directoryToSearch))
            {
                foreach (var subDir in Directory.GetDirectories(directoryToSearch))
                {
                    var directoryName = Path.GetFileName(subDir);
                    if (directoryName.ToLower().Equals("dlls")) continue;

                    foreach (string file in Directory.GetFiles(subDir))
                    {
                        if (!file.ToLower().EndsWith(".dll"))
                        {
                            nestedLevels.Add(file);
                        }
                    }

                    nestedLevels.AddRange(LoadNestedLevelPaths(subDir));
                }
            }

            return nestedLevels;
        }

        public override void LoadNestedItems()
        {
            var levelsToRemove = new List<LevelInfo>();
            CustomFolderInfo parent = null;

            // We want to look here in the event the levels are not already cached.  But if they ARE properly cached, they'll be all nested levels...
            // So for now filter this down to only ones at the root.
            foreach (var level in LevelManager.Instance.CustomLevels.Where(x => Path.GetDirectoryName(x.path) == SaveManager.Instance.CustomLevelsDir))
            {
                if (string.IsNullOrEmpty(level.path) || !level.path.StartsWith(SaveManager.Instance.CustomLevelsDir)) continue;

                // Check to ensure the file is still on disk.  
                if (File.Exists(level.path))
                {
                    AddItem(level, ref parent);
                }
                else
                {
                    // If it's not still on disk, there's a chance user moved it into a folder while the game was running.
                    levelsToRemove.Add(level);
                }
            }

            if (levelsToRemove.Any())
            {
                foreach (var level in levelsToRemove)
                {
                    LevelManager.Instance.CustomLevels.Remove(level);
                }
            }

            foreach (var path in LoadNestedLevelPaths())
            {
                if (string.IsNullOrEmpty(path) || !path.StartsWith(SaveManager.Instance.CustomLevelsDir)) continue;

                var levelSubPath = path.Replace(SaveManager.Instance.CustomLevelsDir + '\\', string.Empty);

                if (string.IsNullOrEmpty(levelSubPath)) continue;

                var folders = levelSubPath.Split('\\').ToList();
                if (folders == null || !folders.Any()) continue;

                parent = null;
                if (folders.Count == 1)
                {
                    // This level is at the root
                    AddItem(LevelManager.Instance.LevelInfoForPath(path), ref parent);
                    continue;
                }

                parent = null;
                for (int i = 0; i < folders.Count; i++)
                {
                    var folder = folders.ElementAt(i);
                    if (folder == null) continue;

                    if (folder == folders.Last())
                    {
                        AddItem(LevelManager.Instance.LevelInfoForPath(path), ref parent);
                    }
                    else
                    {
                        AddFolder<CustomLevelFolderInfo>(folder, path, ref parent);
                    }
                }
            }

            Traverse.Create(LevelManager.Instance).Method("InitializeCustomLevels").GetValue();

            NestedItems = SortList(NestedItems);
        }

        public override List<ICustomInfo> SortList(List<ICustomInfo> levels)
        {
            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((LevelSortMethod)CurrentSort).ToString());

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
            FindObjectOfType<LevelSelectionController>()?.listView?.UpdateList();
        }

        public override void OnNextSort<T>()
        {
            base.OnNextSort<LevelSortMethod>();

            EventSystem.current.SetSelectedGameObject(null);
            FindObjectOfType<LevelSelectionController>()?.listView?.UpdateList();
        }
    }
}
