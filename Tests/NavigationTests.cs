﻿using DemoApp;
using NUnit.Framework;
using UserFlow;

namespace Tests
{
	public class NavigationTests : UserTest<App>
	{
		[Test]
		public void TestNavigationStack()
		{
			OpenMenu("Navigation");
			ShouldSee("Navigation demo");

			Tap("PushAsync");
			ShouldSee("Navigation demo >");

			Tap("PushAsync");
			ShouldSee("Navigation demo > >");

			Tap("PopAsync");
			ShouldSee("Navigation demo >");
		}

		[Test]
		public void TestModalStack()
		{
			OpenMenu("Navigation");
			ShouldSee("Navigation demo");

			Tap("PushModalAsync");
			ShouldSee("Navigation demo ^");

			Tap("PushModalAsync");
			ShouldSee("Navigation demo ^ ^");

			Tap("PopModalAsync");
			ShouldSee("Navigation demo ^");
		}

		[Test]
		public void TestPopToRoot()
		{
			OpenMenu("Navigation");
			ShouldSee("Navigation demo");

			Tap("PushAsync");
			Tap("PushAsync");
			Tap("PushAsync");
			ShouldSee("Navigation demo > > >");

			Tap("PopToRootAsync");
			ShouldSee("Navigation demo");
		}

		[Test]
		public void TestPageAppearingOnAppStart()
		{
			ShouldSee("Log: Appeared");
		}

		[Test]
		public void TestPageDisAppearingOnPushPop()
		{
			Tap("PushAsync");
			ShouldSee("Log: Appeared Disappeared Appeared");

			GoBack();
			ShouldSee("Log: Appeared Disappeared Appeared Disappeared Appeared");
		}

		[Test]
		public void TestPageDisAppearingOnMenuChange()
		{
			OpenMenu("Elements");
			OpenMenu("Navigation");
			ShouldSee("Log: Appeared Disappeared Appeared");
		}

		[Test]
		[Ignore("Not working yet")]
		public void TestDisAppearingPage()
		{
			OpenMenu("Dis-/Appearing");
			ShouldSee("Appeared!");

			OpenMenu("Elements");
			ShouldSee("Disappeared");

			Tap("Ok");
			ShouldSee("Element demo");
		}

		[Test]
		[Ignore("Not working yet")]
		public void TestPopToRootEvent()
		{
			Tap("Dis-/Appearing");
			App.MainPage.Navigation.PopToRootAsync();
			ShouldSee("Disappeared");

			Tap("Ok");
			ShouldSee("Demo page");
		}
	}
}
