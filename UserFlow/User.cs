using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace UserFlow
{
    public partial class User
    {
        readonly Application app;
        readonly Stack<AlertArguments> alerts = new Stack<AlertArguments>();

        public User(Application app)
        {
            this.app = app;

            MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, (page, alert) => {
                alerts.Push(alert);
            });

            WireNavigation();
        }

        public NavigationPage CurrentNavigationPage {
            get {
                return (app.MainPage.Navigation.ModalStack.LastOrDefault() as NavigationPage)
                    ?? (app.MainPage as NavigationPage)
                    ?? (app.MainPage as MasterDetailPage).Detail as NavigationPage;
            }
        }

        ContentPage CurrentPage {
            get {
                var modalStack = app.MainPage.Navigation.ModalStack;
                var currentPage = (modalStack.LastOrDefault() as ContentPage)
                    ?? ((modalStack.LastOrDefault() as NavigationPage)?.CurrentPage as ContentPage);

                var masterDetailPage = app.MainPage as MasterDetailPage;
                if (currentPage == null && masterDetailPage != null && masterDetailPage.IsPresented)
                    currentPage = masterDetailPage.Master as ContentPage;

                var rootPage = masterDetailPage?.Detail ?? app.MainPage;
                if (currentPage == null)
                    currentPage = rootPage.Navigation.NavigationStack.Last() as ContentPage;

                if (currentPage == null)
                    Assert.Fail("CurrentPage is no ContentPage");

                return currentPage;
            }
        }

        public bool CanSee(string text)
        {
            if (alerts.Any()) {
                var alert = alerts.Peek();
                return alert.Title == text
                    || alert.Message == text
                    || alert.Cancel == text
                    || alert.Accept == text;
            }

            return CurrentPage.Find(text).Any();
        }

        public List<Element> Find(string text)
        {
            return CurrentPage.Find(text).Select(i => i.Element).ToList();
        }

        public void Tap(string text)
        {
            if (alerts.Any()) {
                var alert = alerts.Peek();
                if (alert.Accept == text)
                    alert.SetResult(true);
                else if (alert.Cancel == text)
                    alert.SetResult(false);
                else
                    Assert.Fail($"Could not tap \"{text}\" on alert\n{alert}");

                alerts.Pop();
                return;
            }

            var elementInfos = CurrentPage.Find(text);
            Assert.That(elementInfos, Is.Not.Empty, $"Did not find \"{text}\" on current page");
            Assert.That(elementInfos, Has.Count.LessThan(2), $"Found multiple \"{text}\" on current page");

            var elementInfo = elementInfos.First();

            (elementInfo.Element as ToolbarItem)?.Command.Execute(null);
            (elementInfo.Element as Button)?.Command.Execute(null);
            elementInfo.InvokeTap?.Invoke();
        }

        public void Input(string automationId, string text)
        {
            var elements = CurrentPage.Find(automationId).Select(i => i.Element).OfType<VisualElement>().ToList();
            Assert.That(elements, Is.Not.Empty, $"Did not find entry \"{automationId}\" on current page");
            Assert.That(elements, Has.Count.LessThan(2), $"Found multiple entries \"{automationId}\" on current page");

            if (elements.First() is Entry)
                (elements.First() as Entry).Text = text;
            if (elements.First() is Editor)
                (elements.First() as Editor).Text = text;
            if (elements.First() is SearchBar)
                (elements.First() as SearchBar).Text = text;
        }

        public void OpenMenu()
        {
            (app.MainPage as MasterDetailPage).IsPresented = true;
        }

        public void GoBack()
        {
            app.MainPage.SendBackButtonPressed();
        }

        public void Print()
        {
            Console.WriteLine(Render());
        }

        public string Render()
        {
            if (alerts.Any())
                return alerts.Peek().Render();
            else
                return CurrentPage.Render().Trim();
        }
    }
}
