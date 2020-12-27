using System;
using System.Linq;

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
				case (int)Skater.MaleStandard:
				case (int)Skater.FemaleStandard:
				default:
					if (!Enum.TryParse(category.ToString(), out GearCategory gearCategory)) return false;
					return index[1] == (int)gearCategory;
			}
		}
	}
}
