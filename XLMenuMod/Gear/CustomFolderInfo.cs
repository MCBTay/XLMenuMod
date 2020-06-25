using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLMenuMod.Gear.Interfaces;

namespace XLMenuMod.Gear
{
    public class CustomFolderInfo : ICustomGearInfo
    {
        public ICustomGearInfo Parent { get; set; }
        public List<ICustomGearInfo> Children { get; set; }
        public bool IsFavorite { get; set; }
        public string Name { get; set; }
        public DateTime ModifiedDate { get; set; }

        public CustomFolderInfo() 
        {
            Parent = null;
            Children = new List<ICustomGearInfo>();
            IsFavorite = false;
            Name = string.Empty;

            //if (GetName() != "..\\" && !string.IsNullOrEmpty(path))
            //    Size = GetDirectorySize(path);
        }

        private long GetDirectorySize(string directory)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x).ToLower() != "*.dll");

            long directorySize = 0;
            foreach (var file in files)
            {
                directorySize += new FileInfo(file).Length;
            }

            return directorySize;
        }

        public string GetName() { return Name; }

        public DateTime GetModifiedDate() { return GetModifiedDate(true); }
        public DateTime GetModifiedDate(bool ascending)
        {
            DateTime modifiedDate = DateTime.MinValue;

            if (GetName() == "..\\")
                return modifiedDate;

            if (ascending)
            {
                var child = GetOldestChild();
                if (child != null)
                {
                    modifiedDate = child.ModifiedDate;
                }
            }
            else
            {
                var child = GetNewestChild();
                if (child != null)
                {
                    modifiedDate = child.ModifiedDate;
                }
            }

            return modifiedDate;
        }

        //TODO: Refactor this into a way it can be reused between the two systems.
        public ICustomGearInfo GetNewestChild(List<ICustomGearInfo> sourceList = null)
        {
            ICustomGearInfo newestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return newestChild;

            foreach (var child in sourceList)
            {
                if (child is CustomBoardGearInfo)
                {
                    var customLevel = child as CustomBoardGearInfo;

                    if (newestChild == null)
                    {
                        newestChild = customLevel;
                    }
                    else
                    {
                        if (customLevel.ModifiedDate > newestChild.ModifiedDate)
                        {
                            newestChild = customLevel;
                        }
                    }
                }
                else if (child is CustomCharacterGearInfo)
                {
                    var customLevel = child as CustomCharacterGearInfo;

                    if (newestChild == null)
                    {
                        newestChild = customLevel;
                    }
                    else
                    {
                        if (customLevel.ModifiedDate > newestChild.ModifiedDate)
                        {
                            newestChild = customLevel;
                        }
                    }
                }
                else if (child is CustomFolderInfo)
                {
                    var customFolder = child as CustomFolderInfo;

                    if (newestChild == null)
                    {
                        newestChild = GetNewestChild(customFolder.Children);
                    }
                    else
                    {
                        var tempChild = GetNewestChild(customFolder.Children);
                        if (tempChild != null && tempChild.ModifiedDate > newestChild.ModifiedDate)
                        {
                            newestChild = tempChild;
                        }
                    }
                }
            }

            return newestChild;
        }

        public ICustomGearInfo GetOldestChild(List<ICustomGearInfo> sourceList = null)
        {
            ICustomGearInfo oldestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return oldestChild;

            foreach (var child in sourceList)
            {
                if (child is CustomBoardGearInfo)
                {
                    var customLevel = child as CustomBoardGearInfo;

                    if (oldestChild == null)
                    {
                        oldestChild = customLevel;
                    }
                    else
                    {
                        if (customLevel.ModifiedDate < oldestChild.ModifiedDate)
                        {
                            oldestChild = customLevel;
                        }
                    }
                }
                else if (child is CustomCharacterGearInfo)
                {
                    var customLevel = child as CustomCharacterGearInfo;

                    if (oldestChild == null)
                    {
                        oldestChild = customLevel;
                    }
                    else
                    {
                        if (customLevel.ModifiedDate < oldestChild.ModifiedDate)
                        {
                            oldestChild = customLevel;
                        }
                    }
                }
                else if (child is CustomFolderInfo)
                {
                    var customFolder = child as CustomFolderInfo;

                    if (oldestChild == null)
                    {
                        oldestChild = GetNewestChild(customFolder.Children);
                    }
                    else
                    {
                        var tempChild = GetNewestChild(customFolder.Children);
                        if (tempChild != null && tempChild.ModifiedDate < oldestChild.ModifiedDate)
                        {
                            oldestChild = tempChild;
                        }
                    }
                }
            }

            return oldestChild;
        }
    }
}
