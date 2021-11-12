using System;
using System.Linq;
using HarmonyLib;
using XLMenuMod.Utilities.Levels;

namespace XLMenuMod.Utilities.Gear
{
	public static class CategoryHelper
	{
		public static bool IsTypeOf<T>(IndexPath index, T category) where T : Enum
		{
			if (index.depth < 2) return false;

			switch (index[0])
			{
				case (int)Skater.EvanSmith:
					if (!Enum.TryParse(category.ToString(), out EvanSmithGearCategory esCategory)) return false;
					return index[1] == (int)esCategory;
				case (int)Skater.TomAsta:
					if (!Enum.TryParse(category.ToString(), out TomAstaGearCategory taCategory)) return false;
					return index[1] == (int)taCategory;
				case (int)Skater.BrandonWestgate:
					if (!Enum.TryParse(category.ToString(), out BrandonWestgateGearCategory bwCategory)) return false;
					return index[1] == (int)bwCategory;
				case (int)Skater.TiagoLemos:
					if (!Enum.TryParse(category.ToString(), out TiagoLemosGearCategory tlCategory)) return false;
					return index[1] == (int)tlCategory;
				default:
					if (!Enum.TryParse(category.ToString(), out GearCategory gearCategory)) return false;
					return index[1] == (int)gearCategory;
			}
		}

		/// <summary>
		/// Used when changing categories in the gear menu such that the current folder and index path get set appropriately.
		/// </summary>
		public static void WalkUpFolders(this MVCListView instance)
		{
			if (instance.DataSource == null) return;

			var newIndexPath = instance.currentIndexPath;

			if (instance.DataSource is LevelSelectionController && instance.currentIndexPath.depth > 1)
			{
				while (newIndexPath.depth != 1) { newIndexPath = newIndexPath.Up(); }

				Traverse.Create(instance).Property("currentIndexPath").SetValue(newIndexPath);
				CustomLevelManager.Instance.CurrentFolder = null;
			}
			else if (instance.DataSource is GearSelectionController && instance.currentIndexPath.depth > 2)
			{
				while (newIndexPath.depth != 2) { newIndexPath = newIndexPath.Up(); }

				Traverse.Create(instance).Property("currentIndexPath").SetValue(newIndexPath);
				CustomGearManager.Instance.CurrentFolder = null;
			}
		}
	}
}
