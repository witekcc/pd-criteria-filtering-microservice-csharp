using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using CriteriaFilterService.Models;
using System.IO;

namespace CriteriaFilterService.Test
{
    [TestClass]
    public class CriteriaFilterModuleTests
    {
        public CriteriaFilterModuleTests()
        {

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
                with.Header("Content-Type", "application/json");
                with.Body(JsonConvert.SerializeObject(CriteriaGenerator.GenerateCriteria()));              
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
                with.Header("Content-Type", "application/json");
                with.Body(JsonConvert.SerializeObject(CriteriaGenerator.GenerateCriteria()));
            });

            // When
            var result = browser.Post("/criteria", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(JsonConvert.SerializeObject(CriteriaGenerator.GenerateCriteria()));
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
                with.Header("Content-Type", "application/json");
                with.JsonBody<Criteria>(CriteriaGenerator.GenerateCriteria());
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

            var criteria = CriteriaGenerator.GenerateCriteria();

            browser.Post("/criteria", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(JsonConvert.SerializeObject(criteria));
            });

            criteria.Constraints["age"].Exc[0] = "56";
            
            // When
            var result = browser.Put("/criteria/12", with =>
            {
                with.Header("Content-Type", "application/json");
                with.Body(JsonConvert.SerializeObject(criteria));                
            });

            // Then
            Assert.AreEqual(JsonConvert.DeserializeObject<Criteria>(result.Body.AsString()).Constraints["age"].Exc[0], "56");
            
        }
              

        [ClassCleanup]
        public static void DeleteRecord()
        {
            var bootstrapper = new DefaultNancyBootstrapper();
            var browser = new Browser(bootstrapper);
            browser.Delete("/criteria/12");
        }
    }
}
