using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomLevelManager : MonoBehaviour
    {
        public static CustomLevelFolderInfo CurrentFolder { get; set; }
        public static List<ICustomInfo> NestedCustomLevels { get; set; }
        public static float LastSelectedTime { get; set; }
        public static TMP_Text SortLabel;
        public static int CurrentLevelSort { get; set; }

        static CustomLevelManager()
        {
            CurrentFolder = null;
            NestedCustomLevels = new List<ICustomInfo>();
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
            if (level is CustomLevelInfo customLevel)
            {
                CreateOrUpdateLevel(NestedCustomLevels, customLevel, null);
            }
            else
            {
                CreateOrUpdateLevel(NestedCustomLevels, level, null);
            }
        }

        public static void AddLevel(LevelInfo level, ref CustomFolderInfo parent)
        {
            var customLevel = level as CustomLevelInfo;

            var list = parent == null ? NestedCustomLevels : parent.Children;

            CreateOrUpdateLevel(list, customLevel ?? level, parent);
        }

        private static void CreateOrUpdateLevel(List<ICustomInfo> sourceList, LevelInfo levelToAdd, CustomFolderInfo parent)
        {
            ICustomInfo existing = sourceList.FirstOrDefault(x => x.GetName() == levelToAdd.name);

            if (existing == null)
            {
                var newLevel = new CustomLevelInfo(levelToAdd, parent);

                //TODO: Come back to this
                //if (customLevelToAdd != null && customLevelToAdd.Info != null)
                //{
                //    newLevel.Info.UsageCount = customLevelToAdd.Info.UsageCount;
                //    newLevel.Info.LastUsage = customLevelToAdd.Info.LastUsage;
                //}

                sourceList.Add(newLevel.Info);
            }
            else
            {
                //var existingCustom = existing.GetParentObject() as CustomLevelInfo;

                //TODO: Come back to this
                //if (customLevelToAdd != null && customLevelToAdd.Info != null && 
                //    existingCustom != null && existingCustom.Info != null)
                //{
                //    existingCustom.Info.UsageCount = customLevelToAdd.Info.UsageCount;
                //    existingCustom.Info.LastUsage = customLevelToAdd.Info.LastUsage;
                //}
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
                    var newFolder = new CustomLevelFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
                    parent.Children.Add(newFolder.FolderInfo);
                    parent = newFolder.FolderInfo;
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
                    var newFolder = new CustomLevelFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
                    NestedCustomLevels.Add(newFolder.FolderInfo);
                    parent = newFolder.FolderInfo;
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
                levelSelector.LevelCategoryButton.label.SetText(levelSelector.showCustom ? "Custom Maps" : "Official Maps");
            }
            else
            {
                if (Main.BlackSprites != null)
                {
                    levelSelector.LevelCategoryButton.label.spriteAsset = Main.BlackSprites;
                    levelSelector.LevelCategoryButton.label.SetText(CurrentFolder.FolderInfo.GetName().Replace("\\", "<sprite=10> "));
                }
            }
        }


        public static List<ICustomInfo> SortList(List<ICustomInfo> levels)
        {
            List<ICustomInfo> sorted;

            switch (CurrentLevelSort)
            {
                case (int)LevelSortMethod.Recently_Played:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(y => y.GetLastUsage()).ToList();
                    break;
                case (int)LevelSortMethod.Least_Played:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetUsageCount()).ToList();
                    break;
                case (int)LevelSortMethod.Most_Played:
                    sorted = levels.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetUsageCount()).ToList();
                    break;
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
        public static void OnPreviousSort()
        {
            CurrentLevelSort--;

            if (CurrentLevelSort < 0)
                CurrentLevelSort = Enum.GetValues(typeof(LevelSortMethod)).Length - 1;

            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((LevelSortMethod)CurrentLevelSort).ToString());

            if (CurrentFolder != null && CurrentFolder.FolderInfo != null &&
                CurrentFolder.FolderInfo.Children != null && CurrentFolder.FolderInfo.Children.Any())
            {
                CurrentFolder.FolderInfo.Children = SortList(CurrentFolder.FolderInfo.Children);
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

            if (CurrentFolder != null && CurrentFolder.FolderInfo != null && 
                CurrentFolder.FolderInfo.Children != null && CurrentFolder.FolderInfo.Children.Any())
            {
                CurrentFolder.FolderInfo.Children = SortList(CurrentFolder.FolderInfo.Children);
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
