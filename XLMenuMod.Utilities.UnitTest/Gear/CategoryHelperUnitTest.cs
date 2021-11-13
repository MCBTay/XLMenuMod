using System.Collections.Generic;
using FluentAssertions;
using XLMenuMod.Utilities.Gear;
using XLMenuMod.Utilities.UnitTest.Core;
using Xunit;

namespace XLMenuMod.Utilities.UnitTest.Gear
{
    public class CategoryHelperUnitTest
    {
        #region IsTypeOf tests
        [Theory]
        [InlineAutoMoqData(0)]
        [InlineAutoMoqData(1)]
        public void IsTypeOf_InvalidDepth(int depth, IndexPath indexPath)
        {
            indexPath.indices = new int[depth];
            for (int i = 0; i < depth; i++)
            {
                indexPath.indices[i] = i;
            }

            CategoryHelper.IsTypeOf(indexPath, GearCategory.Bottom).Should().BeFalse();
        }

        [Theory]
        [InlineAutoMoqData(Skater.MaleStandard, GearCategory.Bottom, OfficialBrand.Almost)]
        [InlineAutoMoqData(Skater.FemaleStandard, GearCategory.Bottom, OfficialBrand.Almost)]
        [InlineAutoMoqData(Skater.EvanSmith, GearCategory.Shoes, OfficialBrand.Almost)]
        [InlineAutoMoqData(Skater.TomAsta, GearCategory.Top, OfficialBrand.Almost)]
        [InlineAutoMoqData(Skater.BrandonWestgate, GearCategory.Hair, OfficialBrand.Almost)]
        [InlineAutoMoqData(Skater.TiagoLemos, GearCategory.Hair, OfficialBrand.Almost)]
        public void IsTypeOfTests_InvalidValues(int first, int second, OfficialBrand category)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().BeFalse();
        }

        [Theory]
        [InlineAutoMoqData(Skater.MaleStandard, GearCategory.Bottom, GearCategory.Bottom, true)]
        [InlineAutoMoqData(Skater.MaleStandard, GearCategory.Shoes, GearCategory.Bottom, false)]
        [InlineAutoMoqData(Skater.FemaleStandard, GearCategory.Top, GearCategory.Top, true)]
        [InlineAutoMoqData(Skater.FemaleStandard, GearCategory.Hair, GearCategory.Headwear, false)]
        public void IsTypeOfTests_GearCategory(int first, int second, GearCategory category, bool result)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().Be(result);
        }

        [Theory]
        [InlineAutoMoqData(Skater.EvanSmith, EvanSmithGearCategory.Bottom, EvanSmithGearCategory.Bottom, true)]
        [InlineAutoMoqData(Skater.EvanSmith, EvanSmithGearCategory.Shoes, EvanSmithGearCategory.Bottom, false)]
        [InlineAutoMoqData(Skater.EvanSmith, EvanSmithGearCategory.Top, EvanSmithGearCategory.Top, true)]
        public void IsTypeOfTests_EvanSmithGearCategory(int first, int second, EvanSmithGearCategory category, bool result)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().Be(result);
        }

        [Theory]
        [InlineAutoMoqData(Skater.TomAsta, TomAstaGearCategory.Bottom, TomAstaGearCategory.Bottom, true)]
        [InlineAutoMoqData(Skater.TomAsta, TomAstaGearCategory.Shoes, TomAstaGearCategory.Bottom, false)]
        [InlineAutoMoqData(Skater.TomAsta, TomAstaGearCategory.Top, TomAstaGearCategory.Top, true)]
        public void IsTypeOfTests_TomAstaGearCategory(int first, int second, TomAstaGearCategory category, bool result)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().Be(result);
        }

        [Theory]
        [InlineAutoMoqData(Skater.BrandonWestgate, BrandonWestgateGearCategory.Bottom, BrandonWestgateGearCategory.Bottom, true)]
        [InlineAutoMoqData(Skater.BrandonWestgate, BrandonWestgateGearCategory.Shoes, BrandonWestgateGearCategory.Bottom, false)]
        [InlineAutoMoqData(Skater.BrandonWestgate, BrandonWestgateGearCategory.Top, BrandonWestgateGearCategory.Top, true)]
        public void IsTypeOfTests_BrandonWestgateGearCategory(int first, int second, BrandonWestgateGearCategory category, bool result)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().Be(result);
        }

        [Theory]
        [InlineAutoMoqData(Skater.TiagoLemos, TiagoLemosGearCategory.Bottom, TiagoLemosGearCategory.Bottom, true)]
        [InlineAutoMoqData(Skater.TiagoLemos, TiagoLemosGearCategory.Shoes, TiagoLemosGearCategory.Bottom, false)]
        [InlineAutoMoqData(Skater.TiagoLemos, TiagoLemosGearCategory.Top, TiagoLemosGearCategory.Top, true)]
        public void IsTypeOfTests_TiagoLemosGearCategory(int first, int second, TiagoLemosGearCategory category, bool result)
        {
            var index = new IndexPath(new List<int> { first, second });
            CategoryHelper.IsTypeOf(index, category).Should().Be(result);
        }
        #endregion

        #region WalkUpFolders tests

        #endregion
    }
}
