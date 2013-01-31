using System;
using FubuTestingSupport;
using NUnit.Framework;
using OpenQA.Selenium;
using Scoring.Web.Actions.Accounts;
using Scoring.Web.Actions.Athletes;
using Scoring.Web.Actions.Events;
using Scoring.Web.Models;
using SimpleBrowser.WebDriver;

namespace Scoring.IntegrationTests
{
    //TODO: drop down selector isn't working properly
    [TestFixture]
    public class AdminTests
    {
        private IWebDriver driver;
        private const string Events = "Events";
        private const string AdminOps = "Admin Ops";
        private const string Competitors = "Competitors";

        [TestFixtureSetUp]
        public void Init()
        {
            driver = new SimpleBrowserDriver();
            driver.Navigate().GoToUrl("http://localhost:42174/");
            driver.ClickLink("Administrator Sign In");
            driver.FillIn<LoginViewModel>(m => m.Passcode, "Admin204");
            driver.ClickButton();
        }

        [Test]
        public void Event()
        {
            string eventName = "Event One";
            CreateEvent(eventName);
            eventName = EditEvent(eventName);
            DeleteEvent(eventName);
        }

        private string EditEvent(string eventName)
        {
            const string newName = "New Name";
            const string differentDescription = "Different Description";
            driver.ClickLink(Events);
            driver.FindElement(By.Name("Edit" + eventName)).Click();
            driver.FillIn<CreateEventViewModel>(m => m.Event.Name, newName);
            driver.FillIn<CreateEventViewModel>(m => m.Event.Description, differentDescription);
            driver.FillIn<CreateEventViewModel>(m => m.Event.Start, new DateTime(2012, 08, 18, 10, 30, 0));
            driver.FillIn<CreateEventViewModel>(m => m.Event.ScoreType, ScoreType.Time);
            driver.ClickButton();
            driver.ClickLink(Events);
            driver.PageSource.ShouldContain(newName);
            driver.PageSource.ShouldContain(differentDescription);
            return newName;
        }

        private void CreateEvent(string eventName)
        {
            const string eventDescription = "Something Difficult";
            driver.ClickLink(AdminOps);
            driver.ClickLink("New Event");

            driver.FillIn<CreateEventViewModel>(m => m.Event.Number, 1);
            driver.FillIn<CreateEventViewModel>(m => m.Event.Name, eventName);
            driver.FillIn<CreateEventViewModel>(m => m.Event.Description, eventDescription);
            driver.FillIn<CreateEventViewModel>(m => m.Event.Start, new DateTime(2012, 08, 18, 9, 30, 0));
            driver.FillIn<CreateEventViewModel>(m => m.Event.ScoreType, ScoreType.Reps);
            driver.ClickButton();

            driver.ClickLink(Events);
            driver.PageSource.ShouldContain(eventName);
            driver.PageSource.ShouldContain(eventDescription);
        }

        private void DeleteEvent(string eventName)
        {
            driver.ClickLink(Events);
            driver.FindElement(By.Name("Delete" + eventName)).Click();
            driver.PageSource.ShouldNotContain(eventName);
        }

        [Test]
        public void Competitor()
        {
            const string firstName = "Ryan";
            const string lastName = "Weppler";
            CreateCompetitor(firstName);
            DeleteCompetitor(firstName, lastName);
        }

        private void DeleteCompetitor(string firstName, string lastName)
        {
            driver.ClickLink(Competitors);
            var element = driver.FindElement(By.Name("Delete" + firstName + " " + lastName));
            element.Click();
            driver.PageSource.ShouldNotContain(firstName + " " + lastName);
        }

        private void CreateCompetitor(string name)
        {
            driver.ClickLink(AdminOps);
            driver.ClickLink("New Competitor");

            driver.FillIn<CreateAthleteViewModel>(m => m.Athlete.Name, name);
            driver.ClickButton();

            driver.ClickLink(Competitors);
            driver.PageSource.ShouldContain("Ryan Weppler");
        }

        [TestFixtureTearDown]
        public void CleanUp()
        {
            driver.Dispose();
        }
    }
}