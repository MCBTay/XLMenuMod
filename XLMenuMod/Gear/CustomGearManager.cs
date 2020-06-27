using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using XLMenuMod.Gear.Interfaces;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomGearManager : MonoBehaviour
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomInfo> NestedCustomGear { get; set; }
        public static int CurrentGearFilterIndex { get; set; }
        public static float LastSelectedTime { get; set; }
        public static TMP_Text SortLabel;
        public static int CurrentGearSort { get; set; }

        static CustomGearManager()
        {
            CurrentFolder = null;
            NestedCustomGear = new List<ICustomInfo>();
        }

        public static void LoadNestedGear(List<ICharacterCustomizationItem> visibleGear)
        {
            NestedCustomGear.Clear();

            foreach (var gear in visibleGear)
            {
                var singleMaterialGear = gear as GearInfoSingleMaterial;

                // For now all I saw was one texture change per gear type, so assuming first.
                var textureChange = singleMaterialGear?.textureChanges?.FirstOrDefault();
                if (textureChange == null) continue;

                if (string.IsNullOrEmpty(textureChange.texturePath) || !textureChange.texturePath.StartsWith(SaveManager.Instance.CustomGearDir)) continue;

                var textureSubPath = textureChange.texturePath.Replace(SaveManager.Instance.CustomGearDir + '\\', string.Empty);

                if (string.IsNullOrEmpty(textureSubPath)) continue;

                var folders = textureSubPath.Split('\\').ToList();
                if (!folders.Any()) continue;

                if (folders.Count == 1 || Path.GetExtension(folders.First()).ToLower() == ".png")
                {
                    // This gear item is at the root.
                    AddGear(singleMaterialGear);
                    continue;
                }

                CustomFolderInfo parent = null;
                foreach (var folder in folders)
                {
                    if (Path.GetExtension(folder).ToLower() == ".png")
                    {
                        AddGear(singleMaterialGear, ref parent);
                    }
                    else
                    {
                        AddFolder(folder, Path.GetDirectoryName(textureChange.texturePath), ref parent);
                    }
                }
            }

            NestedCustomGear = SortList(NestedCustomGear);
        }

        public static void AddGear(GearInfoSingleMaterial gear)
        {
            ICustomGearInfo customGear = null;

            if (gear is BoardGearInfo boardGear)
            {
                customGear = new CustomBoardGearInfo(boardGear.name, boardGear.type, boardGear.isCustom, boardGear.textureChanges, boardGear.tags);
            }
            else if (gear is CharacterGearInfo charGear)
            {
                customGear = new CustomCharacterGearInfo(charGear.name, charGear.type, charGear.isCustom, charGear.textureChanges, charGear.tags);
            }

            if (customGear != null)
            {
                NestedCustomGear.Add(customGear.Info);
            }
        }

        public static void AddGear(GearInfoSingleMaterial gear, ref CustomFolderInfo parent)
        {
            ICustomGearInfo customGear = null;

            if (gear is BoardGearInfo)
            {
                customGear = new CustomBoardGearInfo(gear.name, gear.type, gear.isCustom, gear.textureChanges, gear.tags);
                customGear.Info.Parent = parent;
            }
            else if (gear is CharacterGearInfo)
            {
                customGear = new CustomCharacterGearInfo(gear.name, gear.type, gear.isCustom, gear.textureChanges, gear.tags);
                customGear.Info.Parent = parent;
            }
            
            if (customGear != null)
            {
                if (parent != null && parent.Children.All(x => x != customGear.Info))
                {
                    parent.Children.Add(customGear.Info);
                }
                else
                {
                    NestedCustomGear.Add(customGear.Info);
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
                    var customFolder = new CustomGearFolderInfo(folderName, path, parent);
                    child = customFolder.FolderInfo;
                    parent.Children.Add(child);
                }

                parent = child;
            }
            else
            {
                var child = NestedCustomGear.FirstOrDefault(x => x.GetName() == folderName && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    var customFolder = new CustomGearFolderInfo(folderName, path, parent);
                    child = customFolder.FolderInfo;
                    NestedCustomGear.Add(child);
                }

                parent = child;
            }
        }

        public static void SetCurrentFolder(CustomFolderInfo folder, bool setGearList = true)
        {
            CurrentFolder = folder;

            if (setGearList) SetGearList();
        }

        public static void MoveUpDirectory()
        {
            CurrentFolder = CurrentFolder.Parent as CustomFolderInfo;

            SetGearList();
        }

        public static void SetGearList()
        {
            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector != null && gearSelector.visibleGear != null)
            {
                gearSelector.visibleGear.Clear();

                if (CurrentFolder == null)
                {
                    gearSelector.visibleGear.AddRange(NestedCustomGear.Select(x => x.GetParentObject() as ICustomGearInfo));
                    gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText("Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel());
                }
                else
                {
                    gearSelector.visibleGear.AddRange(CurrentFolder.Children.Select(x => x.GetParentObject() as ICustomGearInfo));
                    if (Main.BlackSprites != null)
                    {
                        gearSelector.gearTypeFiltering.gearCategoryButton.label.spriteAsset = Main.BlackSprites;
                        gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText(CurrentFolder.GetName().Replace("\\", "<sprite=10> "));
                    }
                }

                gearSelector.listView.UpdateList();
            }
        }

        public static void UpdateLabel()
        {
            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector == null) return;

            if (CurrentFolder == null)
            {
                gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText("Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel());
            }
            else
            {
                if (Main.BlackSprites != null)
                {
                    gearSelector.gearTypeFiltering.gearCategoryButton.label.spriteAsset = Main.BlackSprites;
                    gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText(CurrentFolder.GetName().Replace("\\", "<sprite=10> "));
                }
            }
        }

        public static List<ICustomInfo> SortList(List<ICustomInfo> gear)
        {
            List<ICustomInfo> sorted = null;

            switch (CurrentGearSort)
            {
                case (int)GearSortMethod.Newest:
                    sorted = gear.OrderByDescending(x => x.GetModifiedDate(false)).ToList();
                    break;
                case (int)GearSortMethod.Oldest:
                    sorted = gear.OrderBy(x => x.GetModifiedDate(true)).ToList();
                    break;
                case (int)GearSortMethod.Name_ASC:
                    sorted = gear.OrderBy(x => x.GetName()).ToList();
                    break;
                case (int)GearSortMethod.Name_DESC:
                default:
                    sorted = gear.OrderByDescending(x => x.GetName()).ToList();
                    break;
            }

            return sorted;
        }

        // Not currently used, but already written and may be useful later.
        public static void OnPreviousSort()
        {
            CurrentGearSort--;

            if (CurrentGearSort < 0)
                CurrentGearSort = Enum.GetValues(typeof(GearSortMethod)).Length - 1;

            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((GearSortMethod)CurrentGearSort).ToString());

            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector != null)
            {
                gearSelector.visibleGear.Clear();
                if (CurrentFolder != null && CurrentFolder.Children != null && CurrentFolder.Children.Any())
                {
                    CurrentFolder.Children = SortList(CurrentFolder.Children);
                    var list = CurrentFolder.Children.Select(x => x.GetParentObject() as ICustomGearInfo);
                    gearSelector.visibleGear.AddRange(list);
                }
                else
                {
                    NestedCustomGear = SortList(NestedCustomGear);
                    var list = NestedCustomGear.Select(x => x.GetParentObject() as ICustomGearInfo);
                    gearSelector.visibleGear.AddRange(list);
                }

                Traverse.Create(gearSelector).Method("UpdateList").GetValue();
            }
        }

        public static void OnNextSort()
        {
            CurrentGearSort++;

            if (CurrentGearSort > Enum.GetValues(typeof(GearSortMethod)).Length - 1)
                CurrentGearSort = 0;

            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((GearSortMethod)CurrentGearSort).ToString());

            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector != null)
            {
                gearSelector.visibleGear.Clear();

                if (CurrentFolder?.Children != null && CurrentFolder.Children.Any())
                {
                    CurrentFolder.Children = SortList(CurrentFolder.Children);
                    var list = CurrentFolder.Children.Select(x => x.GetParentObject() as ICustomGearInfo);
                    gearSelector.visibleGear.AddRange(list);
                }
                else
                {
                    NestedCustomGear = SortList(NestedCustomGear);
                    var list = NestedCustomGear.Select(x => x.GetParentObject() as ICustomGearInfo);
                    gearSelector.visibleGear.AddRange(list);
                }

                Traverse.Create(gearSelector).Method("UpdateList").GetValue();
            }
        }
    }
}
