using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CriteriaFilterService.Models;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;

namespace CriteriaFilterService.Test
{

    [TestClass]
    public class FilterModuleTests
    {
        public FilterModuleTests()
        {

        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void LoadCampaigns(TestContext testContext)
        {
            
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            var criteria = CriteriaGenerator.GenerateMultipleCriteria();

            foreach (var cri in criteria)
            {
                browser.Post("/criteria", with =>
                {
                    with.Header("Content-Type", "application/json");
                    with.Body(JsonConvert.SerializeObject(cri));
                });
            }
        }


        [TestMethod]
        public void Should_return_campaign_12_when_filtering_on_age_in_JSON()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            User user = new User() { Age = "23" };

            var result = browser.Get("/filters/12", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("application/json"));
                with.Body(JsonConvert.SerializeObject(user));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("\"12\""), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void Should_return_campaign_12_when_filtering_on_age_in_XML()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            User user = new User() { Age = "23" };

            // When
            var result = browser.Get("/filters/12", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("text/xml"));
                with.Body(JsonConvert.SerializeObject(user));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("<string>12</string>"), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void Should_return_campaigns_12_13_excluding_25_when_filtering_on_age_in_JSON()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
                     
            FilterRequest filter = new FilterRequest() { User = new User() { Age = "23" }, CampaignIds = new List<string>() { "12", "13", "25" } };

            // When
            var result = browser.Get("/filters", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("application/json"));
                with.Body(JsonConvert.SerializeObject(filter));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("\"12\""), true);
            Assert.AreEqual(result.Body.AsString().Contains("\"13\""), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void Should_return_campaigns_13_25_excluding_12_when_filtering_on_age_gender_in_JSON()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
                        
            FilterRequest filter = new FilterRequest() { User = new User() { Age = "19", Gender="M" }, CampaignIds = new List<string>() { "12", "13", "25" } };

            // When
            var result = browser.Get("/filters", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("application/json"));
                with.Body(JsonConvert.SerializeObject(filter));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("\"13\""), true);
            Assert.AreEqual(result.Body.AsString().Contains("\"25\""), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void Should_return_campaigns_13_12_excluding_25_when_filtering_on_age_gender_in_JSON()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            FilterRequest filter = new FilterRequest() { User = new User() { Age = "40", Gender = "F" }, CampaignIds = new List<string>() { "12", "13", "25" } };

            // When
            var result = browser.Get("/filters", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("application/json"));
                with.Body(JsonConvert.SerializeObject(filter));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("\"13\""), true);
            Assert.AreEqual(result.Body.AsString().Contains("\"12\""), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [TestMethod]
        public void Should_return_campaigns_13_12_excluding_25_when_filtering_on_age_gender_without_specifying_campaigns_in_JSON()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            FilterRequest filter = new FilterRequest() { User = new User() { Age = "40", Gender = "F" }, CampaignIds = null };

            // When
            var result = browser.Get("/filters", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Accept(new Nancy.Responses.Negotiation.MediaRange("application/json"));
                with.Body(JsonConvert.SerializeObject(filter));
            });

            // Then
            Assert.AreEqual(result.Body.AsString().Contains("\"13\""), true);
            Assert.AreEqual(result.Body.AsString().Contains("\"12\""), true);
            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);

        }

        [ClassCleanup()]
        public static void DeleteRecords()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            browser.Delete("/criteria/12");
            browser.Delete("/criteria/13");
            browser.Delete("/criteria/25");

        }
    }
}
