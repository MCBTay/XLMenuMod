using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using SkaterXL.Data;
using XLMenuMod.Utilities;
using XLMenuMod.Utilities.Gear;
using XLMenuMod.Utilities.Interfaces;

namespace XLMenuMod.Patches.Gear
{
	public static class GearDatabasePatch
	{
		[HarmonyPatch(typeof(GearDatabase), nameof(GearDatabase.GetGearListAtIndex), new[] { typeof(IndexPath), typeof(bool) }, new[] { ArgumentType.Normal, ArgumentType.Out })]
		public static class GetGearListAtIndexPatch
		{
			static void Postfix(IndexPath index, GearInfo[][][] ___gearListSource, GearInfo[][][] ___customGearListSource, ref GearInfo[] __result)
			{
				// return out if it's not one of the tabs XLMenuMod is aware of.
				if (index[1] < 0 || index[1] > (___gearListSource[index[0]].Length * 2) - 1) return;

				List<ICustomInfo> sourceList = null;

				if (index[1] < ___gearListSource[index[0]].Length)
				{
					var gearToLoad = ___gearListSource[index[0]][index[1]];

					if (CategoryHelper.IsTypeOf(index, GearCategory.Hair))
					{
						CustomGearManager.Instance.LoadNestedHairItems(gearToLoad);
					}
					else if (!CategoryHelper.IsTypeOf(index, GearCategory.SkinTone))
					{
						CustomGearManager.Instance.LoadNestedOfficialItems(gearToLoad);
					}

					if (!CategoryHelper.IsTypeOf(index, GearCategory.SkinTone))
					{
						sourceList = CustomGearManager.Instance.CurrentFolder.HasChildren() ? CustomGearManager.Instance.CurrentFolder.Children : CustomGearManager.Instance.NestedOfficialItems;
					}
				}
				else
				{
					if (index[1] >= ___gearListSource[index[0]].Length)
					{
						var tempIndex = index[1] - ___customGearListSource[index[0]].Length;

						if (tempIndex < ___customGearListSource[index[0]].Length)
							CustomGearManager.Instance.LoadNestedItems(___customGearListSource[index[0]][tempIndex]);
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
			static void Postfix(IndexPath index, GearInfo[][][] ___gearListSource, ref GearInfo __result)
			{
				if (index.depth < 3) return;

				// return out if it's not one of the tabs XLMenuMod is aware of.
				if (index[1] < 0 || index[1] > (___gearListSource[index[0]].Length * 2) - 1) return;

				List<ICustomInfo> sourceList = null;

				if (CustomGearManager.Instance.CurrentFolder.HasChildren())
				{
					sourceList = CustomGearManager.Instance.CurrentFolder.Children;
				}
				else
				{
					if (index[1] < Enum.GetValues(typeof(GearCategory)).Length && !CategoryHelper.IsTypeOf(index, GearCategory.SkinTone))
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
