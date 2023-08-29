using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using RestSharp;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Factories;
using SalesforceAPI.Services.Interfaces;
using SalesforceAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSOLogging;
using SalesforceAPI.Helpers.Interfaces;
using SalesforceAPI.Helpers;
using SalesforceAPI.Models;
using Newtonsoft.Json;
using KellermanSoftware.CompareNetObjects;

namespace SalesforceAPI.Test.Helpers
{
    [TestFixture]
    public class JsonHelperTest
    {
        Mock<ILogger> logger;
        IJsonHelper jsonHelper;

        [SetUp]
        public void Setup()
        {
            logger = new Mock<ILogger>();
            jsonHelper = new JsonHelper(logger.Object);
        }

        [Test]
        public void ConstructPicklistsFromJson_ShouldSuccesfullyReturnMemberPicklistsModel()
        {
            string objectName = Faker.Name.First();
            StreamReader r = new StreamReader("PicklistsResults.json");
            string jsonPicklists = r.ReadToEnd();
            
            var expected = new PicklistFields()
            {
                Roles = new List<string>() { "Administrative Assistant",  "Administrator" },
                Salutations = new List<string>() { "Mr."},
                Departments = new List<string>() { "CS" },
                Credentials = new List<string>() { "RN" },
                ClinicalSpecialties = new List<string>() { "Nuerology" },
                NonClinicalSpecialties = new List<string>() { "Pediatric" }

            };
            var actual = jsonHelper.ConstructPicklistsFromJson(jsonPicklists, UserType.Member);

            CompareLogic comparer = new CompareLogic();
            Assert.IsTrue(comparer.Compare(expected, actual).AreEqual);

        }

        [Test]
        public void ConstructPicklistsFromJson_ShouldSuccesfullyReturnSupplierPicklistsModel()
        {
            string objectName = Faker.Name.First();
            StreamReader r = new StreamReader("PicklistsResults.json");
            string jsonPicklists = r.ReadToEnd();

            var expected = new PicklistFields()
            {
                Roles = new List<string>() { "Administrative Assistant", "Administrator" },
                Salutations = new List<string>() { "Mr." },
                Departments = new List<string>() { "CS" },
                Credentials = new List<string>() { "RN" },
                ClinicalSpecialties = new List<string>(),
                NonClinicalSpecialties = new List<string>()

            };
            var actual = jsonHelper.ConstructPicklistsFromJson(jsonPicklists, UserType.Supplier);

            CompareLogic comparer = new CompareLogic();
            Assert.IsTrue(comparer.Compare(expected, actual).AreEqual);

        }

        [Test]
        public void ConstructPicklistsFromJson_ShouldyReturnEmptyLists_WhenNotExists()
        {
            StreamReader r = new StreamReader("BadPicklistsResults.json");
            string jsonPicklists = r.ReadToEnd();

            var expected = new PicklistFields()
            {
                Roles = new List<string>(),
                Departments = new List<string>(),
                ClinicalSpecialties = new List<string>(),
                NonClinicalSpecialties = new List<string>(),
                Credentials = new List<string>(),
                Salutations = new List<string>()
            };
            var actual = jsonHelper.ConstructPicklistsFromJson(jsonPicklists, UserType.Member);

            CompareLogic comparer = new CompareLogic();
            Assert.IsTrue(comparer.Compare(expected, actual).AreEqual);

        }


    }
}
