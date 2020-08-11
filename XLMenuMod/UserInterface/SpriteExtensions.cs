namespace XLMenuMod.UserInterface
{
	public static class SpriteExtensions
	{
		public static void SetBrandSprite(this MVCListItemView itemView, GearInfo gear)
		{
			if (itemView == null) return;

			string spriteName = gear.name.TrimStart('\\').ToLower();

			if (SpriteHelper.BrandIcons != null) 
				itemView.Label.spriteAsset = SpriteHelper.BrandIcons;

			itemView.SetText(gear.name.Replace("\\", $"<space=30px><size=150%><sprite name=\"{spriteName}\"><size=100%>"), true);
		}

		public static void SetBrandSprite(this MVCListHeaderView headerView, GearInfo gear)
		{
			if (headerView == null) return;

			string spriteName = gear.name.TrimStart('\\').ToLower();

			if (SpriteHelper.BrandIcons != null) 
				headerView.Label.spriteAsset = SpriteHelper.BrandIcons;

			headerView.SetText(gear.name.Replace("\\", $"<space=30px><size=150%><sprite name=\"{spriteName}\"><size=100%>"), true);
		}
	}
}
