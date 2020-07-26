using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLMenuMod.Interfaces;

namespace XLMenuMod
{
    public class CustomFolderInfo : CustomInfo
    {
        [JsonIgnore]
        public List<ICustomInfo> Children { get; set; }

        public override DateTime GetModifiedDate() { return GetModifiedDate(true); }
        public override DateTime GetModifiedDate(bool ascending)
        {
            DateTime modifiedDate = DateTime.MinValue;

            if (GetName() == "..\\" || string.IsNullOrEmpty(Path))
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

        public ICustomInfo GetNewestChild(List<ICustomInfo> sourceList = null)
        {
            ICustomInfo newestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return newestChild;

            foreach (var child in sourceList)
            {
                if (child.IsFolder)
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
                else
                {
                    if (newestChild == null)
                    {
                        newestChild = child;
                    }
                    else
                    {
                        if (child.ModifiedDate > newestChild.ModifiedDate)
                        {
                            newestChild = child;
                        }
                    }
                }
            }

            return newestChild;
        }

        public ICustomInfo GetOldestChild(List<ICustomInfo> sourceList = null)
        {
            ICustomInfo oldestChild = null;

            if (sourceList == null) sourceList = Children;

            if (sourceList == null || !sourceList.Any()) return oldestChild;

            foreach (var child in sourceList)
            {
                if (child.IsFolder)
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
                else
                {
                    if (oldestChild == null)
                    {
                        oldestChild = child;
                    }
                    else
                    {
                        if (child.ModifiedDate < oldestChild.ModifiedDate)
                        {
                            oldestChild = child;
                        }
                    }
                }
            }

            return oldestChild;
        }

        public CustomFolderInfo(string name, string path, CustomFolderInfo parent) : base(name, path, parent, false)
        {
            Children = new List<ICustomInfo>();

            IsFolder = true;

            if (GetName() != "..\\")
            {
	            if (!string.IsNullOrEmpty(path))
	            {
		            Size = GetDirectorySize(path);
                }
                
                Children.Add(new CustomFolderInfo("..\\", Parent == null ? string.Empty : System.IO.Path.GetDirectoryName(Parent.GetPath()), Parent));
            }
        }

        private long GetDirectorySize(string directory)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(x => System.IO.Path.GetExtension(x).ToLower() != "*.dll");

            long directorySize = 0;
            foreach (var file in files)
            {
                directorySize += new FileInfo(file).Length;
            }

            return directorySize;
        }
    }

    public static class CustomFolderInfoExtensions
    {
	    public static bool HasChildren(this CustomFolderInfo folderInfo)
	    {
		    if (folderInfo != null && folderInfo.Children != null && folderInfo.Children.Any()) return true;
		    return false;
	    }
    }
}
