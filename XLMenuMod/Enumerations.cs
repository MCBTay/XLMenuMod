namespace XLMenuMod
{
	public enum FontSizePreset
	{
		Normal = 0,
		Small = 1,
		Smaller = 2,
	}

    /// <summary>
    /// Maps to the TMP_SpriteAsset ControllerIcons found within the game.
    /// </summary>
    public enum ControllerIconSprite
    {
        SWITCH_L = 0,
        XB1_LB = 1,
        SWITCH_R = 2,
        SWITCH_ZL = 3,
        XB1_RB = 4,
        PS4_TouchPad = 5,
        PS4_L1 = 6,
        SWITCH_DOWN = 7,
        SWITCH_LEFT = 8,
        SWITCH_RIGHT = 9,
        SWITCH_UP = 10,
        SWITCH_ZR = 11,
        XB1_B = 12,
        SWITCH_SL = 13,
        XB1_RT = 14,
        PS4_Circle_Button = 15,
        PS4_Triangle_Button = 16,
        SWITCH_A = 17,
        SWITCH_B = 18,
        PS4_L2 = 19,
        XB1_A = 20,
        PS4_R1 = 21,
        SWITCH_SR = 22,
        XB1_Menu = 23,
        PS4_Cross = 24,
        XB1_X = 25,
        SWITCH_Y = 26,
        PS4_R2 = 27,
        PS4_Square = 28,
        PS4_Circle = 29,
        XB1_XboxButton = 30,
        PS4_Options = 31,
        d_pad_down = 32,
        XB1_LT = 33,
        PS4_LeftStick = 34,
        d_pad_left = 35,
        XB1_View = 36,
        PS4_Square_Button = 37,
        PS4_Cross_Button = 38,
        XB1_Y = 39,
        SWITCH_PLUS = 40,
        SWITCH_MINUS = 41,
        SWITCH_X = 42,
        d_pad_right = 43,
        d_pad_up = 44,
        PS4_Triangle = 45,
        PS4_RightStick = 46,
        XB1_LeftStick = 47
    }

    /// <summary>
    /// Maps to the TMP_SpriteAsset ControllerIcons_ReversedOut_Grayish found within the game.
    /// </summary>
    public enum ControllerIconSprite_Gray
    {
	    PS4_Circle,
	    PS4_L1,
	    PS4_L2,
	    PS4_R1,
	    PS4_R2,
	    PS4_TouchPad,
	    XB1_LB,
	    PS4_Square_Button,
	    PS4_Triangle_Button,
	    SWITCH_A,
	    SWITCH_LEFT,
	    SWITCH_UP,
	    XB1_LT,
	    XB1_RT,
	    PS4_Options,
	    XB1_RB,
	    SWITCH_ZL,
	    SWITCH_R,
	    PS4_Cross_Button,
	    PS4_Triangle,
	    XB1_View,
	    SWITCH_DOWN,
	    SWITCH_RIGHT,
	    SWITCH_PLUS,
	    SWITCH_SL,
	    SWITCH_ZR,
	    PS4_Circle_Button,
	    XB1_Y,
	    SWITCH_Y,
	    XB1_Menu,
	    d_pad_down,
	    d_pad_right,
	    PS4_Square,
	    PS4_Cross,
	    SWITCH_B,
	    SWITCH_X,
	    SWITCH_SR,
	    d_pad_left,
	    d_pad_up,
	    XB1_A,
	    SWITCH_L,
	    SWITCH_MINUS,
	    XB1_B,
	    XB1_X,
	    LeftStick,
	    RightStick,
	    PS4_Options_Alt,
    }

    public enum LevelSortMethod
    {
        Name_ASC,
        Name_DESC,
        Filesize_ASC,
        Filesize_DESC,
        Newest,
        Oldest,
        Author_ASC,
        Author_DESC,
        //Most_Played,
        //Least_Played,
        //Recently_Played
    }

    public enum GearSortMethod
    {
        Name_ASC,
        Name_DESC,
        Newest,
        Oldest
    }

    public enum OfficialBrands
    {
	    FourOneOne,
        Almost,
        Blind,
        Bones,
        Crupie,
        Enjoi,
        Etnies, 
	    Grizzly,
	    Independent,
        Lakai, 
	    Old_Friends,
	    Sk8Mafia,
        SOVRN, 
	    DC, 
	    Dickies, 
	    Element,
	    Emerica,
	    eS,
	    Grimple_Stix,
        HUF, 
	    Jenkem,
	    Mob,
        New_Balance,
        Pepper,
        Primitive,
        Ricta,
        Santa_Cruz,
	    Spitfire,
        Nine_Club,
        Thunder,
        Transworld,
        Vans,
	    Venture,
    }
}
