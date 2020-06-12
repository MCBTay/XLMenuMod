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

            var levelsToRemove = new List<LevelInfo>();
            foreach (var level in LevelManager.Instance.CustomLevels)
            {
                if (string.IsNullOrEmpty(level.path) || !level.path.StartsWith(SaveManager.Instance.CustomLevelsDir)) continue;

                // Check to ensure the file is still on disk.  
                if (File.Exists(level.path))
                {
                    AddLevel(level);
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
                var levelSubPath = path.Replace(SaveManager.Instance.CustomLevelsDir + '\\', string.Empty);

                if (string.IsNullOrEmpty(levelSubPath)) continue;

                var folders = levelSubPath.Split('\\').ToList();
                if (folders == null || !folders.Any()) continue;

                if (folders.Count == 1)
                {
                    // This level is at the root
                    AddLevel(LevelManager.Instance.LevelInfoForPath(path));
                    continue;
                }

                CustomFolderInfo parent = null;
                for (int i = 0; i < folders.Count; i++)
                {
                    var folder = folders.ElementAt(i);
                    if (folder == null) continue;

                    if (folder == folders.Last())
                    {
                        AddLevel(LevelManager.Instance.LevelInfoForPath(path), ref parent);
                    }
                    else
                    {
                        AddFolder(folder, path, ref parent);
                    }
                }
            }

            NestedCustomLevels = NestedCustomLevels.OrderBy(x => x.GetName()).ToList();
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

        public static void AddFolder(string folder, string path, ref CustomFolderInfo parent)
        {
            var newFolder = new CustomFolderInfo { name = $"\\{folder}", Parent = parent };
            newFolder.Children.Add(new CustomFolderInfo { name = "..\\", Parent = newFolder.Parent });

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
    }
}
