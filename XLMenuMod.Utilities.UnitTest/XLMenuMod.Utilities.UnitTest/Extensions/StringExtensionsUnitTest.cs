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
		[InlineAutoMoqData("C:\\users\\username\\documents\\skaterxl\\gear\\folder1\\folder2\\somefile.png", "C:\\users\\username\\documents\\skaterxl\\maps", false)]
		public void IsSubPathOfTests(string path, string baseDirPath, bool expected)
		{
			path.IsSubPathOf(baseDirPath).Should().Be(expected);
		}
	}
}
