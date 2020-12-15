using System;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using XLMenuMod.Utilities;
using XLMenuMod.Utilities.Gear;
using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Patches.Gear
{
	public class GearDatabasePatch
	{
		[HarmonyPatch(typeof(GearDatabase), nameof(GearDatabase.GetGearListAtIndex), new[] { typeof(IndexPath), typeof(bool) }, new[] { ArgumentType.Normal, ArgumentType.Out })]
		public static class GetGearListAtIndexPatch
		{
			static void Postfix(GearDatabase __instance, IndexPath index, ref GearInfo[] __result)
			{
				var officialGear = Traverse.Create(__instance).Field("gearListSource").GetValue<GearInfo[][][]>();

				// return out if it's not one of the tabs XLMenuMod is aware of.
				if (index[1] < 0 || index[1] > (officialGear[index[0]].Length * 2) - 1) return;

				List<ICustomInfo> sourceList = null;

				if (index[1] < officialGear[index[0]].Length)
				{
					var gearToLoad = officialGear[index[0]][index[1]];

					if (index[1] == (int)GearCategory.Hair)
					{
						CustomGearManager.Instance.LoadNestedHairItems(gearToLoad);
					}
					else if (index[1] != (int)GearCategory.SkinTone)
					{
						CustomGearManager.Instance.LoadNestedOfficialItems(gearToLoad);
					}

					if (index[1] != (int)GearCategory.SkinTone)
					{
						sourceList = CustomGearManager.Instance.CurrentFolder.HasChildren() ? CustomGearManager.Instance.CurrentFolder.Children : CustomGearManager.Instance.NestedOfficialItems;
					}
				}
				else
				{
					if (index[1] >= officialGear[index[0]].Length)
					{
						var customGear = Traverse.Create(__instance).Field("customGearListSource").GetValue<GearInfo[][][]>();

						var tempIndex = index[1] - customGear[index[0]].Length;

						if (tempIndex < customGear[index[0]].Length)
							CustomGearManager.Instance.LoadNestedItems(customGear[index[0]][tempIndex]);
					}

					sourceList = CustomGearManager.Instance.CurrentFolder.HasChildren() ? CustomGearManager.Instance.CurrentFolder.Children : CustomGearManager.Instance.NestedItems;
				}

				if (sourceList == null) return;

				__result = sourceList.Select(x => x.GetParentObject() as GearInfo).ToArray();
			}
		}

		[HarmonyPatch(typeof(GearDatabase), nameof(GearDatabase.GetGearAtIndex), new[] { typeof(IndexPath), typeof(bool) }, new [] { ArgumentType.Normal, ArgumentType.Out})]
		public static class GetGearAtIndexPatch
		{
			static void Postfix(GearDatabase __instance, IndexPath index, ref GearInfo __result)
			{
				if (index.depth < 3) return;

				var officialGear = Traverse.Create(__instance).Field("gearListSource").GetValue<GearInfo[][][]>();
				// return out if it's not one of the tabs XLMenuMod is aware of.
				if (index[1] < 0 || index[1] > (officialGear[index[0]].Length * 2) - 1) return;

				List<ICustomInfo> sourceList = null;

				if (CustomGearManager.Instance.CurrentFolder.HasChildren())
				{
					sourceList = CustomGearManager.Instance.CurrentFolder.Children;
				}
				else
				{
					if (index[1] < Enum.GetValues(typeof(GearCategory)).Length && index[1] != (int)GearCategory.SkinTone)
					{
						sourceList = CustomGearManager.Instance.NestedOfficialItems;
					}
					else if (index[1] >= Enum.GetValues(typeof(GearCategory)).Length)
					{
						sourceList = CustomGearManager.Instance.NestedItems;
					}
				}

				if (sourceList == null) return;

				if (index.LastIndex < 0 || index.LastIndex >= sourceList.Count) return;

				var customInfo = sourceList.ElementAt(index.LastIndex);

				if (customInfo.GetParentObject() is CustomBoardGearInfo customBoardGearInfo) __result = customBoardGearInfo;
				else if (customInfo.GetParentObject() is CustomCharacterGearInfo customCharacterGearInfo) __result = customCharacterGearInfo;
				else if (customInfo.GetParentObject() is CustomCharacterBodyInfo customCharacterBodyInfo) __result = customCharacterBodyInfo;
				else if (customInfo.GetParentObject() is CustomGearFolderInfo customGearFolderInfo) __result = customGearFolderInfo;
			}
		}
	}
}
