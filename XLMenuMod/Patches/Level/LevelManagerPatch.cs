using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XLMenuMod.Levels;

namespace XLMenuMod.Patches.Level
{
    public class LevelManagerPatch
    {
        [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadCachedLevels))]
        public static class LoadCachedLevelsPatch
        {
            static bool Prefix(DateTime ___lastHashingTime)
            {
                if (LevelManager.Instance.IsHashing)
                    return false;
                string str = SaveManager.Instance.LoadCustomLevelListCache();
                if (str == null)
                    return false;
                ___lastHashingTime = File.GetLastWriteTime(SaveManager.Instance.CachedLevelsPath);

                LevelManager.Instance.CustomLevels.Clear();
                LevelManager.Instance.CustomLevels.AddRange(JsonConvert.DeserializeObject<List<CustomLevelInfo>>(str));

                Debug.Log($"XLMenuMod: Loaded CustomLevel Cache with {LevelManager.Instance.CustomLevels.Count} Levels last updated on from {___lastHashingTime.ToShortDateString()} at {___lastHashingTime.ToShortTimeString()}");

                return false;
            }
        }

        /// <summary>
        /// Some levels have custom script assemblies that go with them.  Find if an assembly exists that matches the maps file name, and load it if soo.
        /// </summary>
        [HarmonyPatch(typeof(LevelManager), nameof(LevelManager.LoadLevelScene))]
        public static class LoadLevelScenePatch
        {
            static void Prefix(LevelInfo level)
            {
                var path = Path.GetDirectoryName(level.path);
                var file = Path.Combine(path, level.name + ".dll");
                if (Directory.Exists(path) && File.Exists(file))
                {
                    // Because other mods may load the assembly, check to ensure that it's not already loaded.
                    var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => !x.IsDynamic && Path.GetFileName(x.Location).StartsWith(level.name));
                    if (assembly == null)
                    {
                        Assembly.LoadFile(file);
                    }
                }
            }
        }
    }
}
