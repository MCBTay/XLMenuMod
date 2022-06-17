using FluentAssertions;
using XLMenuMod.Utilities.UnitTest.Core;
using XLMenuMod.Utilities.UserInterface;
using Xunit;

namespace XLMenuMod.Utilities.UnitTest.UserInterface
{
    public class UserInterfaceHelperUnitTest
    {
        #region GetFontSize tests

        [Theory]
        [InlineAutoMoqData(FontSizePreset.Normal, 36)]
        [InlineAutoMoqData(FontSizePreset.Small, 30)]
        [InlineAutoMoqData(FontSizePreset.Smaller, 24)]
        public void GetFontSizeTests(FontSizePreset fontSize, int expectedFontSize)
        {
            var actual = UserInterfaceHelper.Instance.GetFontSize(fontSize);

            actual.Should().Be(expectedFontSize);
        }
        #endregion
    }
}
