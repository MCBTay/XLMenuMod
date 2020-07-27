using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using XLMenuMod.Gear;
using XLMenuMod.Interfaces;
using XLMenuMod.Levels;

namespace XLMenuMod
{
    public abstract class CustomManager : MonoBehaviour
    {
        public CustomFolderInfo CurrentFolder { get; set; }
        public List<ICustomInfo> NestedItems { get; set; }
        public float LastSelectedTime { get; set; }
        public TMP_Text SortLabel;
        public int CurrentSort { get; set; }

        public CustomManager()
        {
            CurrentFolder = null;
            NestedItems = new List<ICustomInfo>();
        }

        public virtual void AddItem<T>(T source, List<ICustomInfo> sourceList, ref CustomFolderInfo parent) where T : class
        {
	        if (source is LevelInfo level)
            {
                var existing = sourceList.FirstOrDefault(x => x.GetName() == level.name);
             
                if (existing == null) sourceList.Add(new CustomLevelInfo(level, parent).Info);
            }
            else if (source is GearInfoSingleMaterial gear)
            {
                var existing = sourceList.FirstOrDefault(x => x.GetName() == gear.name);

                if (existing == null)
                {
                    if (source is BoardGearInfo)
                    {
                        var customGear = new CustomBoardGearInfo(gear.name, gear.type, gear.isCustom, gear.textureChanges, gear.tags);
                        customGear.Info.Parent = parent;
                        sourceList.Add(customGear.Info);
                    }
                    else if (source is CharacterGearInfo)
                    {
                        var customGear = new CustomCharacterGearInfo(gear.name, gear.type, gear.isCustom, gear.textureChanges, gear.tags);
                        customGear.Info.Parent = parent;
                        sourceList.Add(customGear.Info);
                    }
                }
            }
        }

        public virtual void AddFolder<T>(string folder, string path, List<ICustomInfo> sourceList, ref CustomFolderInfo parent) where T : ICustomFolderInfo
        {
            string folderName = $"\\{folder}";

            var child = sourceList.FirstOrDefault(x => x.GetName() == folderName && x is CustomFolderInfo) as CustomFolderInfo;
            if (child == null)
            {
                ICustomFolderInfo newFolder;

                if (typeof(T) == typeof(CustomLevelFolderInfo))
                {
                    newFolder = new CustomLevelFolderInfo($"\\{folder}", Path.GetDirectoryName(path), parent);
                }
                else if (typeof(T) == typeof(CustomGearFolderInfo))
                {
                    newFolder = new CustomGearFolderInfo($"\\{folder}", path, parent);
                }
                else return;

                sourceList.Add(newFolder.FolderInfo);
                parent = newFolder.FolderInfo;
            }
            else
            {
                parent = child;
            }
        }

        public virtual void OnPreviousSort<T>() where T : struct, IConvertible
        {
            CurrentSort--;

            if (CurrentSort < 0)
                CurrentSort = Enum.GetValues(typeof(T)).Length - 1;

            if (CurrentFolder?.Children != null && CurrentFolder.Children.Any())
            {
                CurrentFolder.Children = SortList(CurrentFolder.Children);
            }
            else
            {
                NestedItems = SortList(NestedItems);
            }
        }

        public virtual void OnNextSort<T>() where T : struct, IConvertible
        {
            CurrentSort++;

            if (CurrentSort > Enum.GetValues(typeof(T)).Length - 1)
                CurrentSort = 0;

            if (CurrentFolder.HasChildren())
            {
                CurrentFolder.Children = SortList(CurrentFolder.Children);
            }
            else
            {
                NestedItems = SortList(NestedItems);
            }
        }

        public abstract List<ICustomInfo> SortList(List<ICustomInfo> sourceList);
        public abstract void LoadNestedItems(object[] objectsToLoad = null);
    }
}
