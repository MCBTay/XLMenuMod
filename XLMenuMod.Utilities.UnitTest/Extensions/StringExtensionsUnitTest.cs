using System;
using FluentAssertions;
using XLMenuMod.Utilities.Extensions;
using XLMenuMod.Utilities.UnitTest.Core;
using Xunit;

namespace XLMenuMod.Utilities.UnitTest.Extensions
{
	public class StringExtensionsUnitTest
	{
		[Theory]
		[InlineAutoMoqData("C:\\users\\username\\documents\\skaterxl\\gear\\folder1\\folder2\\somefile.png", "C:\\users\\username\\documents\\skaterxl\\gear", true)]
		[InlineAutoMoqData("C:/users/username/documents/skaterxl/gear/folder1/folder2/somefile.png", "C:/users/username/documents/skaterxl/gear", true)]
		[InlineAutoMoqData("C:\\users\\username\\documents\\skaterxl\\gear\\folder1\\folder2\\somefile.png", "C:\\users\\username\\documents\\skaterxl\\maps", false)]
		[InlineAutoMoqData("C:/users/username/documents/skaterxl/gear/folder1/folder2/somefile.png", "C:/users/username/documents/skaterxl/maps", false)]
		public void IsSubPathOfTests(string path, string baseDirPath, bool expected)
		{
			path.IsSubPathOf(baseDirPath).Should().Be(expected);
        }

        [Fact]
        public void WithEnding_NullString()
        {
            string test = null;
            string expected = "otherString";

            test.WithEnding(expected).Should().Be(expected);
        }

        [Fact]
        public void Right_NullValue()
        {
            string test = null;

            Action act = () => test.Right(5);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Right_LessThanZeroLength()
        {
            string test = "value";

            Action act = () => test.Right(-1);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
