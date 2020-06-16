using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityModManagerNet;
using XLMenuMod.Levels.Interfaces;

namespace XLMenuMod.Levels
{
    public class CustomFolderInfo : LevelInfo, ICustomLevelInfo
    {
        [JsonIgnore]
        public ICustomLevelInfo Parent { get; set; }

        [JsonIgnore]
        public List<ICustomLevelInfo> Children { get; set; }

        /// <summary>
        /// Will be the sum of the directory's contents.
        /// </summary>
        [JsonIgnore]
        public long Size { get; set; }

        [JsonIgnore]
        public int PlayCount { get; set; }

        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }

        [JsonIgnore]
        public bool IsFavorite { get; set; }

        public string GetName() { return name; }
        public long GetSize() { return Size; }
        public string GetHash() { return hash; }

        public DateTime GetModifiedDate() { return GetModifiedDate(true); }
        public DateTime GetModifiedDate(bool ascending) 
        {
            DateTime modifiedDate = DateTime.MinValue;

            if (GetName() == "..\\" || string.IsNullOrEmpty(path))
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

        public CustomLevelInfo GetNewestChild(List<ICustomLevelInfo> sourceList = null)
        {
            CustomLevelInfo newestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return newestChild;

            foreach (var child in sourceList)
            {
                if (child is CustomLevelInfo)
                {
                    var customLevel = child as CustomLevelInfo;
                
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

        public CustomLevelInfo GetOldestChild(List<ICustomLevelInfo> sourceList = null)
        {
            CustomLevelInfo oldestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return oldestChild;

            foreach (var child in sourceList)
            {
                if (child is CustomLevelInfo)
                {
                    var customLevel = child as CustomLevelInfo;

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



        public int GetPlayCount(List<ICustomLevelInfo> source = null)
        {
            int playCount = 0;

            if (source == null) source = Children;

            if (source != null && source.Any())
            {
                foreach (var level in source)
                {
                    if (level is CustomFolderInfo)
                    {
                        var customFolder = level as CustomFolderInfo;
                        playCount += GetPlayCount(customFolder.Children);
                    }
                    else if (level is CustomLevelInfo)
                    {
                        var customLevel = level as CustomLevelInfo;
                        playCount += customLevel.PlayCount;
                    }
                }
            }

            return playCount;
        }

        public CustomFolderInfo(string name, string path, ICustomLevelInfo parent)
        {
            this.name = name;
            this.path = path;
            Parent = parent;
            Children = new List<ICustomLevelInfo>();

            if (GetName() != "..\\" && !string.IsNullOrEmpty(path))
                Size = GetDirectorySize(path);
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
    }
}
