using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CriteriaFilterService.Test
{
    [TestClass]
    public class CriteriaFilterServiceTests
    {
        private Criteria GenerateCriteria()
        {
            Criteria criteria = new Criteria()
            {
                CampaignId = "12",
                CampaignUUID = "A02AECA1-C7DD-4FC5-ADDF-ED5486F77A09",
                Constraints = new
                {
                    phone = new { inc = new[] { "5000000000-6000000000" }, exc = new[] { "5555555551" } },
                    age = new { inc = new[] { "21-35" }, exc = new[] { "26" } },
                    zip = new { inc = new[] { "12345-12954", "54313" }, exc = new[] { "" } }
                }
            };

            return criteria;
        }

        [TestMethod]
        public void Should_return_404_when_not_passing_an_id()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get("/criteria", with =>
            {
                with.HttpRequest();
            });

            // Then
            Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
        }

        [TestMethod]
        public void Should_return_ok_when_criteria_is_created_sucessfully()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            browser.Delete("/criteria/12");

            // When
            var result = browser.Post("/criteria", with =>
            {
                with.JsonBody<Criteria>(GenerateCriteria());                
            });

            // Then
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        [TestMethod]
        public void Should_return_conflict_when_criteria_creation_is_attempted_but_criteria_already_exists()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            browser.Post("/criteria", with =>
            {
                with.JsonBody<Criteria>(GenerateCriteria());
            });

            // When
            var result = browser.Post("/criteria", with =>
            {
                with.JsonBody<Criteria>(GenerateCriteria());
            });

            // Then
            Assert.AreEqual(HttpStatusCode.Conflict, result.StatusCode);
        }

        [TestMethod]
        public void Should_return_NoContent_when_criteria_is_deleted_sucessfully()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            
            browser.Post("/criteria", with =>
            {
                with.JsonBody<Criteria>(GenerateCriteria());
            });

            // When
            var result = browser.Delete("/criteria/12");

            // Then
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [TestMethod]
        public void Should_return_json_object_when_update_criteria_is_sucessful()
        {
            // Given
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);

            var criteria = GenerateCriteria();

            browser.Post("/criteria", with =>
            {
                with.JsonBody<Criteria>(criteria);
            });

            criteria.Constraints = new { age = new { inc = new[] { "55-60" }, exc = new[] { "56" } } };
            
            // When
            var result = browser.Put("/criteria/12", with =>
            {
                with.JsonBody<Criteria>(criteria);
            });

            // Then
            Assert.AreEqual(result.Body.DeserializeJson<Criteria>().Constraints["Age"]["Exc"][0], "56");
            
        }

        [ClassCleanup]
        private void DeleteRecord()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            browser.Delete("/criteria/12");
        }
    }
}
