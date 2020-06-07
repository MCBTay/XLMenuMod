using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelManager : LevelManager
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomLevelInfo> NestedCustomLevels { get; set; }
        public static List<ICustomLevelInfo> OriginalCustomLevels { get; set; }

        static CustomLevelManager()
        {
            CurrentFolder = null;
            NestedCustomLevels = new List<ICustomLevelInfo>();
            OriginalCustomLevels = new List<ICustomLevelInfo>();
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
            Instance.UpdateCustomMaps();

            NestedCustomLevels.Clear();

            foreach (var level in Instance.CustomLevels)
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
                        AddFolder(folder, ref parent);
                    }
                }
            }

            var orderedLevels = NestedCustomLevels.OrderBy(x => x.GetName());
            NestedCustomLevels = orderedLevels.ToList();

            if (CurrentFolder == null && NestedCustomLevels.Any())
            {
                OriginalCustomLevels.Clear();
                OriginalCustomLevels.AddRange(NestedCustomLevels);
            }

            var levelSelector = FindObjectOfType<LevelSelectionController>();
            if (levelSelector != null)
            {
                levelSelector.Items.Clear();
                foreach (var customLevel in NestedCustomLevels)
                {
                    if (customLevel is CustomFolderInfo)
                        levelSelector.Items.Add(customLevel as CustomFolderInfo);
                    else if (customLevel is CustomLevelInfo)
                        levelSelector.Items.Add(customLevel as CustomLevelInfo);
                }
                levelSelector.UpdateList();
            }
        }

        public static void AddLevel(LevelInfo level)
        {
            NestedCustomLevels.Add(new CustomLevelInfo(level));
        }

        public static void AddLevel(LevelInfo level, ref CustomFolderInfo parent)
        {
            var customLevel = new CustomLevelInfo(level, parent);

            if (parent != null && !parent.Children.Any(x => x == customLevel))
            {
                parent.Children.Add(customLevel);
            }
            else if (!NestedCustomLevels.Any(x => x == customLevel))
            {
                NestedCustomLevels.Add(customLevel);
            }
        }

        public static void AddFolder(string folder, ref CustomFolderInfo parent)
        {
            var newFolder = new CustomFolderInfo { name = $"\\{folder}", Parent = parent, LevelInfo = new LevelInfo { name = $"\\{folder}" } };
            newFolder.Children.Add(new CustomFolderInfo { name = "..\\", Parent = newFolder.Parent, LevelInfo = new LevelInfo { name = $"..\\" } });

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

        public static void SetCurrentFolder(CustomFolderInfo folder)
        {
            CurrentFolder = folder;

            SetLevelList();
        }

        public static void MoveUpDirectory()
        {
            CurrentFolder = CurrentFolder.Parent as CustomFolderInfo;

            SetLevelList();
        }

        public static void SetLevelList()
        {
            var levelSelector = FindObjectOfType<LevelSelectionController>();
            if (levelSelector != null)
            {
                levelSelector.Items.Clear();

                if (CurrentFolder == null)
                {
                    levelSelector.Items.AddRange(NestedCustomLevels.Select(x => x.GetLevelInfo()).ToList());
                    levelSelector.LevelCategoryButton.label.text = levelSelector.showCustom ? "Custom Maps" : "Official Maps";
                }
                else
                {
                    levelSelector.Items.AddRange(CurrentFolder.Children.Select(x => x.GetLevelInfo()).ToList());
                    levelSelector.LevelCategoryButton.label.text = CurrentFolder.GetName();
                }

                

                levelSelector.UpdateList();
            }
        }
    }
}
