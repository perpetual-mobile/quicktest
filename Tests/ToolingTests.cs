using System;
using System.Linq;
using DemoApp;
using NUnit.Framework;
using UserFlow;

namespace Tests
{
    public class ToolingTests : IntegrationTest<App>
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();

            OpenMenu("Navigation");
        }

        [Test]
        public void TestShouldSee()
        {
            ShouldSee("PushAsync", "PopAsync");
        }

        [Test]
        public void TestTap()
        {
            Tap("PushAsync", "PopAsync");
            ShouldSee("Navigation");
        }

        [Test]
        public void TestFind()
        {
            var elements = Find("PushAsync");
            Assert.That(elements, Has.Count.EqualTo(1));
            Assert.That(elements.First(), Is.TypeOf<DemoButton>());
            Assert.That((elements.First() as DemoButton).Text, Is.EqualTo("PushAsync"));
        }

        [Test]
        public void TestDelay()
        {
            OpenMenu("Elements");
            Tap("Countdown");
            ShouldNotSee("Countdown");
            var time = DateTime.Now;
            After(2).ShouldSee("2");
            After(1).Tap("1");
            After(1).ShouldNotSee("1");
            Assert.That(DateTime.Now - time, Is.GreaterThan(TimeSpan.FromSeconds(1)));
        }
    }
}
