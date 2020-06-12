using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomGearManager : MonoBehaviour
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomGearInfo> NestedCustomGear { get; set; }
        public static int CurrentGearFilterIndex { get; set; }
        public static float LastSelectedTime { get; set; }

        static CustomGearManager()
        {
            CurrentFolder = null;
            NestedCustomGear = new List<ICustomGearInfo>();
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
                if (folders == null || !folders.Any()) continue;

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
                        AddFolder(folder, ref parent);
                    }
                }
            }

            NestedCustomGear = NestedCustomGear.OrderBy(x => x.GetName()).ToList();
        }

        public static void AddGear(GearInfoSingleMaterial gear)
        {
            ICustomGearInfo customGear = null;

            if (gear is BoardGearInfo)
            {
                var boardGear = gear as BoardGearInfo;
                customGear = new CustomBoardGearInfo(boardGear.name, boardGear.type, boardGear.isCustom, boardGear.textureChanges, boardGear.tags)
                {
                    Parent = null
                };
            }
            else if (gear is CharacterGearInfo)
            {
                var characterGear = gear as CharacterGearInfo;
                customGear = new CustomCharacterGearInfo(characterGear.name, characterGear.type, characterGear.isCustom, characterGear.textureChanges, characterGear.tags)
                {
                    Parent = null
                };
            }

            if (customGear != null)
            {
                NestedCustomGear.Add(customGear);
            }
        }

        public static void AddGear(GearInfoSingleMaterial gear, ref CustomFolderInfo parent)
        {
            ICustomGearInfo customGear = null;

            if (gear is BoardGearInfo)
            {
                var boardGear = gear as BoardGearInfo;
                customGear = new CustomBoardGearInfo(boardGear.name, boardGear.type, boardGear.isCustom, boardGear.textureChanges, boardGear.tags)
                {
                    Parent = parent
                };
            }
            else if (gear is CharacterGearInfo)
            {
                var characterGear = gear as CharacterGearInfo;
                customGear = new CustomCharacterGearInfo(characterGear.name, characterGear.type, characterGear.isCustom, characterGear.textureChanges, characterGear.tags)
                {
                    Parent = parent
                };
            }
            
            if (customGear != null)
            {
                if (parent != null && !parent.Children.Any(x => x == customGear))
                {
                    parent.Children.Add(customGear);
                }
                else
                {
                    NestedCustomGear.Add(customGear);
                }
            }
        }

        public static void AddFolder(string folder, ref CustomFolderInfo parent)
        {
            var newFolder = new CustomFolderInfo { Name = $"\\{folder}", Parent = parent };
            newFolder.Children.Add(new CustomFolderInfo { Name = "..\\", Parent = newFolder.Parent });

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
                var child = NestedCustomGear.FirstOrDefault(x => x.GetName() == newFolder.GetName() && x is CustomFolderInfo) as CustomFolderInfo;
                if (child == null)
                {
                    NestedCustomGear.Add(newFolder);
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

            SetGearList();
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
                    gearSelector.visibleGear.AddRange(NestedCustomGear);
                    gearSelector.gearTypeFiltering.gearCategoryButton.label.text = "Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel();
                }
                else
                {
                    gearSelector.visibleGear.AddRange(CurrentFolder.Children);
                    gearSelector.gearTypeFiltering.gearCategoryButton.label.text = CurrentFolder.GetName();
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
                gearSelector.gearTypeFiltering.gearCategoryButton.label.text = "Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel();
            }
            else
            {
                gearSelector.gearTypeFiltering.gearCategoryButton.label.text = CurrentFolder.GetName();
            }
        }
    }
}
