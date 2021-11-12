﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using System.Diagnostics.CodeAnalysis;

namespace XLMenuMod.Utilities.UnitTest.Core
{
	[ExcludeFromCodeCoverage]
	public class AutoMoqDataAttribute : AutoDataAttribute
	{
		public AutoMoqDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
		{

		}
	}
}
