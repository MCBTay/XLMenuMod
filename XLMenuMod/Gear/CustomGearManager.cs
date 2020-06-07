using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomGearManager : MonoBehaviour
    {
        public static CustomFolderInfo CurrentFolder { get; set; }
        public static List<ICustomGearInfo> NestedCustomGear { get; set; }
        public static List<ICharacterCustomizationItem> OriginalCustomGear { get; set; }
        public static int CurrentGearFilterIndex { get; set; }

        static CustomGearManager()
        {
            CurrentFolder = null;
            NestedCustomGear = new List<ICustomGearInfo>();
            OriginalCustomGear = new List<ICharacterCustomizationItem>();
        }

        public static void AddGear(GearInfoSingleMaterial gear)
        {
            ICustomGearInfo customGear = null;

            if (gear is BoardGearInfo)
            {
                var boardGear = gear as BoardGearInfo;
                customGear = new CustomBoardGearInfo(boardGear.name, boardGear.type, boardGear.isCustom, boardGear.textureChanges, boardGear.tags)
                {
                    GearInfo = boardGear,
                    Parent = null
                };
            }
            else if (gear is CharacterGearInfo)
            {
                var characterGear = gear as CharacterGearInfo;
                customGear = new CustomBoardGearInfo(characterGear.name, characterGear.type, characterGear.isCustom, characterGear.textureChanges, characterGear.tags)
                {
                    GearInfo = characterGear,
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
                    GearInfo = boardGear,
                    Parent = parent
                };
            }
            else if (gear is CharacterGearInfo)
            {
                var characterGear = gear as CharacterGearInfo;
                customGear = new CustomBoardGearInfo(characterGear.name, characterGear.type, characterGear.isCustom, characterGear.textureChanges, characterGear.tags)
                {
                    GearInfo = characterGear,
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
    }
}
