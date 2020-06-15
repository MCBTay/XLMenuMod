using Harmony12;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelManager : MonoBehaviour
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomLevelInfo> NestedCustomLevels { get; set; }
        public static float LastSelectedTime { get; set; }
        public static CategoryButton SortCategoryButton { get; set; }
        public static int CurrentLevelSort { get; set; }

        static CustomLevelManager()
        {
            CurrentFolder = null;
            NestedCustomLevels = new List<ICustomLevelInfo>();
        }

        public static List<string> LoadNestedLevelPaths(string directoryToSearch = null)
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

        public static void LoadNestedLevels()
        {
            NestedCustomLevels.Clear();

            foreach (var level in LevelManager.Instance.CustomLevels)
            {
                if (string.IsNullOrEmpty(level.path) || !level.path.StartsWith(SaveManager.Instance.CustomLevelsDir)) continue;

                var levelSubPath = level.path.Replace(SaveManager.Instance.CustomLevelsDir + '\\', string.Empty);

                if (string.IsNullOrEmpty(levelSubPath)) continue;

                var folders = levelSubPath.Split('\\').ToList();
                if (folders == null || !folders.Any()) continue;

                if (folders.Count == 1)
                {
                    // This level is at the root
                    AddLevel(level);
                    continue;
                }

                CustomFolderInfo parent = null;
                for (int i = 0; i < folders.Count; i++)
                {
                    var folder = folders.ElementAt(i);
                    if (folder == null) continue;

                    if (folder == folders.Last())
                    {
                        AddLevel(level, ref parent);
                    }
                    else
                    {
                        AddFolder(folder, level.path, ref parent);
                    }
                }
            }

            Traverse.Create(LevelManager.Instance).Method("InitializeCustomLevels").GetValue();
        }

        public static void AddLevel(LevelInfo level)
        {
            if (level is CustomLevelInfo)
            {
                var customLevel = level as CustomLevelInfo;
                NestedCustomLevels.Add(customLevel);
            }
            else
            {
                NestedCustomLevels.Add(new CustomLevelInfo(level));
            }
        }

        public static void AddLevel(LevelInfo level, ref CustomFolderInfo parent)
        {
            CustomLevelInfo customLevel = null;

            if (level is CustomLevelInfo)
            {
                customLevel = level as CustomLevelInfo;
            }
            else
            {
                customLevel = new CustomLevelInfo(level, parent);
            }

            if (customLevel == null) return;

            if (parent != null && !parent.Children.Any(x => x == customLevel))
            {
                parent.Children.Add(customLevel);
            }
            else if (!NestedCustomLevels.Any(x => x == customLevel))
            {
                NestedCustomLevels.Add(customLevel);
            }
        }

        public static void AddFolder(string folder, string path, ref CustomFolderInfo parent)
        {
            var newFolder = new CustomFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
            newFolder.Children.Add(new CustomFolderInfo("..\\", parent == null ? string.Empty : Path.GetDirectoryName(parent.path), newFolder.Parent));

            if (parent != null)
            {
                var child = parent.Children.FirstOrDefault(x => x.GetName() == newFolder.GetName() && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    parent.Children.Add(newFolder);
                    parent = newFolder;
                }
                else
                {
                    parent = child;
                }
            }
            else
            {
                var child = NestedCustomLevels.FirstOrDefault(x => x.GetName() == newFolder.GetName() && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    NestedCustomLevels.Add(newFolder);
                    parent = newFolder;
                }
                else
                {
                    parent = child;
                }
            }
        }

        public static void UpdateLabel()
        {
            var levelSelector = FindObjectOfType<LevelSelectionController>();
            if (levelSelector == null) return;

            if (CurrentFolder == null)
            {
                levelSelector.LevelCategoryButton.label.text = levelSelector.showCustom ? "Custom Maps" : "Official Maps";
            }
            else
            {
                levelSelector.LevelCategoryButton.label.text = CurrentFolder.GetName();
            }
        }

        public static List<ICustomLevelInfo> SortList(List<ICustomLevelInfo> levels)
        {
            List<ICustomLevelInfo> sorted = null;

            switch (CurrentLevelSort)
            {
                case (int)LevelSortMethods.Least_Played:
                    sorted = levels.OrderBy(x => x.GetPlayCount()).ToList();
                    break;
                case (int)LevelSortMethods.Most_Played:
                    sorted = levels.OrderByDescending(x => x.GetPlayCount()).ToList();
                    break;
                case (int)LevelSortMethods.Newest:
                    sorted = levels.OrderByDescending(x => x.GetModifiedDate(false)).ToList();
                    break;
                case (int)LevelSortMethods.Oldest:
                    sorted = levels.OrderBy(x => x.GetModifiedDate(true)).ToList();
                    break;
                case (int)LevelSortMethods.Filesize_ASC:
                    sorted = levels.OrderBy(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethods.Filesize_DESC:
                    sorted = levels.OrderByDescending(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethods.Name_ASC:
                    sorted = levels.OrderBy(x => x.GetName()).ToList();
                    break;
                case (int)LevelSortMethods.Name_DESC:
                default:
                    sorted = levels.OrderByDescending(x => x.GetName()).ToList();
                    break;
            }

            return sorted;
        }

        public enum LevelSortMethods
        {
            Name_ASC,
            Name_DESC,
            Filesize_ASC,
            Filesize_DESC,
            Newest,
            Oldest,
            Most_Played,
            Least_Played,
        }
    }
}
