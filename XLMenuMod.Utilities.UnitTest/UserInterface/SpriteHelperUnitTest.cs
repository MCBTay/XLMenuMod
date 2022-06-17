using FluentAssertions;
using UnityEngine;
using XLMenuMod.Utilities.UnitTest.Core;
using XLMenuMod.Utilities.UserInterface;
using Xunit;

namespace XLMenuMod.Utilities.UnitTest.UserInterface
{
    public class SpriteHelperUnitTest
    {
        [Theory]
        [InlineAutoMoqData(RuntimePlatform.WindowsEditor, ControllerIconSpriteGray.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSpriteGray.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSpriteGray.PS4_Triangle_Button, "Dual Shock")]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSpriteGray.PS4_Triangle_Button, "DualShock")]
        [InlineAutoMoqData(RuntimePlatform.PS4, ControllerIconSpriteGray.PS4_Triangle_Button)]
        [InlineAutoMoqData(RuntimePlatform.XboxOne, ControllerIconSpriteGray.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.Switch, ControllerIconSpriteGray.SWITCH_X)]
        public void GetSpriteIndex_YButton_Gray_Test(
            RuntimePlatform platform,
            ControllerIconSpriteGray sprite,
            string joystickName = "joystick")
        {
            SpriteHelper sut = new SpriteHelper();
            var actual = sut.GetSpriteIndex_YButton_Gray(platform, joystickName);
            actual.Should().Be((int)sprite);
        }

        [Theory]
        [InlineAutoMoqData(RuntimePlatform.WindowsEditor, ControllerIconSprite.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.PS4_Triangle_Button, "Dual Shock")]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.PS4_Triangle_Button, "DualShock")]
        [InlineAutoMoqData(RuntimePlatform.PS4, ControllerIconSprite.PS4_Triangle_Button)]
        [InlineAutoMoqData(RuntimePlatform.XboxOne, ControllerIconSprite.XB1_Y)]
        [InlineAutoMoqData(RuntimePlatform.Switch, ControllerIconSprite.SWITCH_X)]
        public void GetSpriteIndex_YButton_Test(
            RuntimePlatform platform,
            ControllerIconSprite sprite,
            string joystickName = "joystick")
        {
            SpriteHelper sut = new SpriteHelper();
            var actual = sut.GetSpriteIndex_YButton(platform, joystickName);
            actual.Should().Be((int)sprite);
        }

        [Theory]
        [InlineAutoMoqData(RuntimePlatform.WindowsEditor, ControllerIconSprite.XB1_X)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.XB1_X)]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.PS4_Square_Button, "Dual Shock")]
        [InlineAutoMoqData(RuntimePlatform.WindowsPlayer, ControllerIconSprite.PS4_Square_Button, "DualShock")]
        [InlineAutoMoqData(RuntimePlatform.PS4, ControllerIconSprite.PS4_Square_Button)]
        [InlineAutoMoqData(RuntimePlatform.XboxOne, ControllerIconSprite.XB1_X)]
        [InlineAutoMoqData(RuntimePlatform.Switch, ControllerIconSprite.SWITCH_Y)]
        public void GetSpriteIndex_XButton_Test(
            RuntimePlatform platform,
            ControllerIconSprite sprite,
            string joystickName = "joystick")
        {
            SpriteHelper sut = new SpriteHelper();
            var actual = sut.GetSpriteIndex_XButton(platform, joystickName);
            actual.Should().Be((int)sprite);
        }
    }
}
