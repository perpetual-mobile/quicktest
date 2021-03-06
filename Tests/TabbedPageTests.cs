﻿using DemoApp;
using NUnit.Framework;
using QuickTest;

namespace Tests
{
    public class TabbedPageTests : QuickTest<App>
    {
        string expectedLog;

        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            Launch(new App());
            OpenMenu("TabbedPage");

            expectedLog = "A(FlyoutPage) A(NavigationPage) A(Navigation) D(Navigation) D(NavigationPage) A(NavigationPage) A(TabbedPage) A(Tab A) ";
            Assert.That(App.PageLog, Is.EqualTo(expectedLog));
        }

        [Test]
        public void SwitchTab()
        {
            ShouldSee("TabbedPage", "Tab A", "Tab B", "This is content on tab A");
            ShouldNotSee("This is content on tab B", "ToolbarItem");

            Tap("Tab B");

            ShouldSee("This is content on tab B");
            ShouldNotSee("This is content on tab A");
            Assert.That(App.PageLog, Is.EqualTo(expectedLog += "D(Tab A) A(Tab B) "));

            Tap("ToolbarItem");
            ShouldSee("ToolbarItem tapped");
        }

        [Test]
        public void ModalPageOverTabbedPage()
        {
            Assert.That(App.PageLog, Is.EqualTo(expectedLog));
            ShouldSee("TabbedPage");

            Tap("Open ModalPage");
            ShouldSee("This is a modal page");
            ShouldNotSee("TabbedPage");
            Assert.That(App.PageLog, Is.EqualTo(expectedLog += "D(Tab A) D(TabbedPage) D(NavigationPage) D(FlyoutPage) A(Modal) "));
            Tap("Close");
            Assert.That(App.PageLog, Is.EqualTo(expectedLog += "D(Modal) A(FlyoutPage) A(NavigationPage) A(TabbedPage) A(Tab A) "));
        }

        [Test]
        public void NavigationFromTabbedPage()
        {
            Assert.That(App.PageLog, Is.EqualTo(expectedLog));
            ShouldSee("TabbedPage");

            Tap("Open Subpage");
            ShouldSee("This is a sub page");
            ShouldNotSee("TabbedPage");
            Assert.That(App.PageLog, Is.EqualTo(expectedLog += "D(Tab A) D(TabbedPage) A(Subpage) "));
            GoBack();
            Assert.That(App.PageLog, Is.EqualTo(expectedLog += "D(Subpage) A(TabbedPage) A(Tab A) "));
        }
    }
}
