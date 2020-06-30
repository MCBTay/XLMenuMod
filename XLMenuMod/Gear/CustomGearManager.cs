using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.EventSystems;
using XLMenuMod.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomGearManager : CustomManager
    {
        private static CustomGearManager _instance;
        public static CustomGearManager Instance => _instance ?? (_instance = new CustomGearManager());

        public static int CurrentGearFilterIndex { get; set; }

        public override void LoadNestedItems()
        {
            NestedItems.Clear();

            foreach (var gear in GearSelectionController.Instance.visibleGear)
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

                CustomFolderInfo parent = null;
                if (folders.Count == 1 || Path.GetExtension(folders.First()).ToLower() == ".png")
                {
                    // This gear item is at the root.
                    AddItem(singleMaterialGear, ref parent);
                    continue;
                }

                parent = null;
                foreach (var folder in folders)
                {
                    if (Path.GetExtension(folder).ToLower() == ".png")
                    {
                        AddItem(singleMaterialGear, ref parent);
                    }
                    else
                    {
                        AddFolder<CustomGearFolderInfo>(folder, Path.GetDirectoryName(textureChange.texturePath), ref parent);
                    }
                }
            }

            NestedItems = SortList(NestedItems);
        }

        public void SetCurrentFolder(CustomFolderInfo folder, bool showCustom)
        {
            CurrentFolder = folder;

            SetGearList(showCustom);
        }

        public void SetGearList(bool showCustom)
        {
            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector != null && gearSelector.visibleGear != null)
            {
                if (showCustom)
                {
                    gearSelector.visibleGear.Clear();

                    if (CurrentFolder == null)
                    {
                        gearSelector.visibleGear.AddRange(NestedItems.Select(x => x.GetParentObject() as ICharacterCustomizationItem));
                        gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText("Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel());
                    }
                    else
                    {
                        gearSelector.visibleGear.AddRange(CurrentFolder.Children.Select(x => x.GetParentObject() as ICharacterCustomizationItem));
                        if (Main.BlackSprites != null)
                        {
                            gearSelector.gearTypeFiltering.gearCategoryButton.label.spriteAsset = Main.BlackSprites;
                            gearSelector.gearTypeFiltering.gearCategoryButton.label.SetText(CurrentFolder.GetName().Replace("\\", "<sprite=10> "));
                        }
                    }
                }
                
                gearSelector.listView.UpdateList();
            }
        }

        public override void UpdateLabel()
        {
            var gearSelector = FindObjectOfType<GearSelectionController>();
            if (gearSelector == null) return;

            UserInterfaceHelper.SetCategoryButtonLabel(ref gearSelector.gearTypeFiltering.gearCategoryButton.label, CurrentFolder.GetName(), "Custom " + gearSelector.gearTypeFiltering.GearFilters[CurrentGearFilterIndex].GetLabel(), CurrentFolder == null);
        }

        public override List<ICustomInfo> SortList(List<ICustomInfo> gear)
        {
            UserInterfaceHelper.SetSortLabelText(ref SortLabel, ((GearSortMethod)CurrentSort).ToString());

            List<ICustomInfo> sorted;

            switch (CurrentSort)
            {
                case (int)GearSortMethod.Newest:
                    sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetModifiedDate(false)).ToList();
                    break;
                case (int)GearSortMethod.Oldest:
                    sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetModifiedDate(true)).ToList();
                    break;
                case (int)GearSortMethod.Name_ASC:
                    sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenBy(x => x.GetName()).ToList();
                    break;
                case (int)GearSortMethod.Name_DESC:
                default:
                    sorted = gear.OrderBy(x => x.GetName() != "..\\").ThenByDescending(x => x.GetName()).ToList();
                    break;
            }

            return sorted;
        }

        public override void OnPreviousSort<T>()
        {
            base.OnPreviousSort<GearSortMethod>();

            EventSystem.current.SetSelectedGameObject(null);
            Traverse.Create(FindObjectOfType<GearSelectionController>())?.Method("UpdateList").GetValue();
        }

        public override void OnNextSort<T>()
        {
            base.OnNextSort<GearSortMethod>();

            EventSystem.current.SetSelectedGameObject(null);
            Traverse.Create(FindObjectOfType<GearSelectionController>())?.Method("UpdateList").GetValue();
        }
    }
}
