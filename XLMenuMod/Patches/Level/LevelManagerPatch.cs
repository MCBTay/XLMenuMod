using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace XLMenuMod.Patches.Level
{
	public static class LevelManagerPatch
    {
        /// <summary>
        /// Some levels have custom script assemblies that go with them.  Find if an assembly exists that matches the maps file name, and load it if soo.
        /// </summary>
        [HarmonyPatch(typeof(LevelManager), "LoadLevelScene")]
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
