namespace XLMenuMod.UserInterface
{
	public static class SpriteExtensions
	{
		public static void SetBrandSprite(this MVCListItemView itemView, GearInfo gear)
		{
			if (itemView == null) return;

			string spriteName = gear.name.TrimStart('\\').ToLower();

			//if (spriteName == "411") spriteName = "fouroneone";
			//else if (spriteName.ToLower() == "és") spriteName = "es";
			//else if (spriteName.ToLower() == "the_nine_club") spriteName = "nine_club";

			if (SpriteHelper.BrandIcons != null) itemView.Label.spriteAsset = SpriteHelper.BrandIcons;

			itemView.SetText(gear.name.Replace("\\", $"<space=30px><size=150%><sprite name=\"{spriteName}\"><size=100%>"), true);
		}

		public static void SetBrandSprite(this MVCListHeaderView headerView, GearInfo gear)
		{
			string spriteName = gear.name.TrimStart('\\').ToLower();

			//if (spriteName == "411") spriteName = "fouroneone";
			//else if (spriteName.ToLower() == "és") spriteName = "es";
			//else if (spriteName.ToLower() == "the_nine_club") spriteName = "nine_club";

			if (SpriteHelper.BrandIcons != null) headerView.Label.spriteAsset = SpriteHelper.BrandIcons;

			headerView.SetText(gear.name.Replace("\\", $"<space=30px><size=150%><sprite name=\"{spriteName}\"><size=100%>"), true);
		}
	}
}
