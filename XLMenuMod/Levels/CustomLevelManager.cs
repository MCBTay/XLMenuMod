using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelManager : MonoBehaviour
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomLevelInfo> NestedCustomLevels { get; set; }
        public static float LastSelectedTime { get; set; }
        public static TMP_Text SortLabel;
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
            var levelsToRemove = new List<LevelInfo>();

            // We want to look here in the event the levels are not already cached.  But if they ARE properly cached, they'll be all nested levels...
            // So for now filter this down to only ones at the root.
            foreach (var level in LevelManager.Instance.CustomLevels.Where(x => Path.GetDirectoryName(x.path) == SaveManager.Instance.CustomLevelsDir))
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
                if (string.IsNullOrEmpty(path) || !path.StartsWith(SaveManager.Instance.CustomLevelsDir)) continue;

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

            Traverse.Create(LevelManager.Instance).Method("InitializeCustomLevels").GetValue();
        }

        public static void AddLevel(LevelInfo level)
        {
            if (level is CustomLevelInfo)
            {
                var customLevel = level as CustomLevelInfo;
                CreateOrUpdateLevel(NestedCustomLevels, level, customLevel, null);
            }
            else
            {
                CreateOrUpdateLevel(NestedCustomLevels, level, null, null);
            }
        }

        public static void AddLevel(LevelInfo level, ref CustomFolderInfo parent)
        {
            var customLevel = level as CustomLevelInfo;

            if (parent == null)
            {
                CreateOrUpdateLevel(NestedCustomLevels, level, customLevel, parent);
            }
            else
            {
                CreateOrUpdateLevel(parent.Children, level, customLevel, parent);
            }
        }

        private static void CreateOrUpdateLevel(List<ICustomLevelInfo> sourceList, LevelInfo levelToAdd, CustomLevelInfo customLevelToAdd, CustomFolderInfo parent)
        {
            LevelInfo existing = null;

            if (customLevelToAdd != null)
            {
                existing = sourceList.FirstOrDefault(x => x.GetName() == customLevelToAdd.GetName()) as CustomLevelInfo;
            }
            else
            {
                existing = sourceList.FirstOrDefault(x => x.GetName() == levelToAdd.name) as LevelInfo;
            }
            
            if (existing == null)
            {
                sourceList.Add(new CustomLevelInfo(levelToAdd, parent)
                {
                    PlayCount = customLevelToAdd == null ? 0 : customLevelToAdd.PlayCount,
                    LastPlayTime = customLevelToAdd == null ? DateTime.MinValue : customLevelToAdd.LastPlayTime
                });
            }
            else
            {
                if (existing is CustomLevelInfo)
                {
                    var existingCustom = existing as CustomLevelInfo;
                    existingCustom.PlayCount = customLevelToAdd == null ? 0 : customLevelToAdd.PlayCount;
                    existingCustom.LastPlayTime = customLevelToAdd == null ? DateTime.MinValue : customLevelToAdd.LastPlayTime;
                }
            }
        }

        public static void AddFolder(string folder, string path, ref CustomFolderInfo parent)
        {
            var folderName = $"\\{folder}";

            if (parent != null)
            {
                var child = parent.Children.FirstOrDefault(x => x.GetName() == folderName && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    var newFolder = new CustomFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
                    newFolder.Children.Add(new CustomFolderInfo("..\\", parent == null ? string.Empty : Path.GetDirectoryName(parent.path), newFolder.Parent));

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
                var child = NestedCustomLevels.FirstOrDefault(x => x.GetName() == folderName && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    var newFolder = new CustomFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
                    newFolder.Children.Add(new CustomFolderInfo("..\\", parent == null ? string.Empty : Path.GetDirectoryName(parent.path), newFolder.Parent));

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
                case (int)LevelSortMethod.Recently_Played:
                    sorted = levels.OrderBy(x => x.GetLastPlayTime()).ToList();
                    break;
                case (int)LevelSortMethod.Least_Played:
                    sorted = levels.OrderBy(x => x.GetPlayCount()).ToList();
                    break;
                case (int)LevelSortMethod.Most_Played:
                    sorted = levels.OrderByDescending(x => x.GetPlayCount()).ToList();
                    break;
                case (int)LevelSortMethod.Newest:
                    sorted = levels.OrderByDescending(x => x.GetModifiedDate(false)).ToList();
                    break;
                case (int)LevelSortMethod.Oldest:
                    sorted = levels.OrderBy(x => x.GetModifiedDate(true)).ToList();
                    break;
                case (int)LevelSortMethod.Filesize_ASC:
                    sorted = levels.OrderBy(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethod.Filesize_DESC:
                    sorted = levels.OrderByDescending(x => x.Size).ToList();
                    break;
                case (int)LevelSortMethod.Name_ASC:
                    sorted = levels.OrderBy(x => x.GetName()).ToList();
                    break;
                case (int)LevelSortMethod.Name_DESC:
                default:
                    sorted = levels.OrderByDescending(x => x.GetName()).ToList();
                    break;
            }

            return sorted;
        }           

        // Not currently used, but already written and may be useful later.
        public static void OnPreviousSort()
        {
            CurrentLevelSort--;

            if (CurrentLevelSort < 0)
                CurrentLevelSort = Enum.GetValues(typeof(LevelSortMethod)).Length - 1;

            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((LevelSortMethod)CurrentLevelSort).ToString());

            if (CurrentFolder != null && CurrentFolder.Children != null && CurrentFolder.Children.Any())
            {
                CurrentFolder.Children = SortList(CurrentFolder.Children);
            }
            else
            {
                NestedCustomLevels = SortList(NestedCustomLevels);
            }

            var levelSelector = FindObjectOfType<LevelSelectionController>();

            EventSystem.current.SetSelectedGameObject(null);
            if (levelSelector != null)
                levelSelector.UpdateList();
        }

        public static void OnNextSort()
        {
            CurrentLevelSort++;

            if (CurrentLevelSort > Enum.GetValues(typeof(LevelSortMethod)).Length - 1)
                CurrentLevelSort = 0;

            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((LevelSortMethod)CurrentLevelSort).ToString());

            if (CurrentFolder != null && CurrentFolder.Children != null && CurrentFolder.Children.Any())
            {
                CurrentFolder.Children = SortList(CurrentFolder.Children);
            }
            else
            {
                NestedCustomLevels = SortList(NestedCustomLevels);
            }

            var levelSelector = FindObjectOfType<LevelSelectionController>();

            EventSystem.current.SetSelectedGameObject(null);
            if (levelSelector != null)
                levelSelector.UpdateList();
        }
    }
}
