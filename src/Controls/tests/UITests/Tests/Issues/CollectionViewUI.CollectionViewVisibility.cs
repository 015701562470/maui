﻿using NUnit.Framework;
using UITest.Appium;
using UITest.Core;

namespace Microsoft.Maui.AppiumTests
{
	public class CollectionViewVisibilityUITests : _IssuesUITest
	{
		const string Success = "Success";
		const string Show = "Show";

		public CollectionViewVisibilityUITests(TestDevice device)
			: base(device)
		{
		}

		public override string Issue => "iOS application suspended at UICollectionViewFlowLayout.PrepareLayout() when using IsVisible = false";

		// InitiallyInvisbleCollectionViewSurvivesiOSLayoutNonsense(src\Compatibility\ControlGallery\src\Issues.Shared\Issue12714.cs)
		[Test]
		[IgnoreOnIOS("This test is failing, requires research.")]
		[IgnoreOnMac("This test is failing, requires research.")]
		[IgnoreOnWindows]
		public void InitiallyInvisbleCollectionViewSurvivesiOSLayoutNonsense()
		{
			App.WaitForElement(Show);
			App.Click(Show);
			App.WaitForNoElement(Success);
		}
	}
}