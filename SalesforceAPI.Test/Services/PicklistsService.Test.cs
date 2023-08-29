using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using SalesforceAPI.Helpers.Interfaces;
using SalesforceAPI.Services;
using SalesforceAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSOLogging;
using SalesforceAPI.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using KellermanSoftware.CompareNetObjects;

namespace SalesforceAPI.Test.Services
{
    [TestFixture]
    public class PicklistsServiceTest
    {
        IPicklistsService picklistsService;
        Mock<ISalesforceAPIService> salesforceAPIService;
        Mock<IJsonHelper> jsonHelper;
        IMemoryCache memoryCache;
        Mock<ILogger> logger;
        IConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            salesforceAPIService = new Mock<ISalesforceAPIService>();
            jsonHelper = new Mock<IJsonHelper>();
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            logger = new Mock<ILogger>();
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.localhost.json").Build(); 
            picklistsService = new PicklistsService(salesforceAPIService.Object, jsonHelper.Object, memoryCache, logger.Object, configuration);
        }


        [Test]
        public void GetAllPicklists_ShouldReturnEmptyPicklistsAndLogError_WhenExceptionThrown()
        {
            var expected = new PicklistFields();
            picklistsService = new PicklistsService(salesforceAPIService.Object, jsonHelper.Object, null, logger.Object, configuration);
            
            var actual = picklistsService.GetAllPicklists(0);

            CompareLogic comparer = new CompareLogic();
            var compareResult = comparer.Compare(expected, actual);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);
        }

        [Test]
        public void GetAllPicklists_ShouldReturnPicklists()
        {

            PicklistFields expected = CreatePicklistsRecords();

            UserType userType = UserType.Member;

            memoryCache.Set($"{userType}-picklists", expected);

            var actual = picklistsService.GetAllPicklists(userType);

            CompareLogic comparer = new CompareLogic();
            var compareResult = comparer.Compare(expected, actual);
            Assert.IsTrue(compareResult.AreEqual, compareResult.DifferencesString);


        }

        [Test]
        public void RefreshPicklists_ShouldRefreshPicklistsInCache()
        {
            var expectedCachedPicklists = CreatePicklistsRecords();
            string jsonPicklists = JsonConvert.SerializeObject(expectedCachedPicklists);
            UserType userType = UserType.Member;
            salesforceAPIService.Setup(x => x.GetPicklists(userType)).Returns(jsonPicklists).Verifiable();
            jsonHelper.Setup(x => x.ConstructPicklistsFromJson(jsonPicklists, userType)).Returns(expectedCachedPicklists).Verifiable();

            picklistsService.RefreshPicklists(userType);

            var actualCachedPicklists = memoryCache.Get($"{userType}-picklists");
            Assert.AreEqual(expectedCachedPicklists, actualCachedPicklists);
        }

        [Test]
        public void RefreshPicklists_ShouldNotSetOrResetCache_WhenPicklistsEmpty()
        {
            UserType userType = UserType.Member;

            salesforceAPIService.Setup(x => x.GetPicklists(userType)).Returns(It.IsAny<string>()).Verifiable();
            jsonHelper.Setup(x => x.ConstructPicklistsFromJson(It.IsAny<string>(), userType)).Throws(It.IsAny<Exception>).Verifiable();

            picklistsService.RefreshPicklists(userType);

            salesforceAPIService.VerifyAll();
            jsonHelper.VerifyAll();
            var actualCachedPicklists = memoryCache.Get($"{userType}-picklists");
            Assert.AreEqual(null, actualCachedPicklists);
        }
        
        private PicklistFields CreatePicklistsRecords(int limit = 20)
        {
            PicklistFields picklists = new PicklistFields()
            {
                Roles = new List<string>(),
                Departments = new List<string>(),
                ClinicalSpecialties = new List<string>(),
                NonClinicalSpecialties = new List<string>(),
                Credentials = new List<string>(),
                Salutations = new List<string>(),
            };
            for (int i = 0; i < limit; i++)
            {
                picklists.Roles.Add(Faker.Name.First());
                picklists.Salutations.Add(Faker.Name.First());
                picklists.ClinicalSpecialties.Add(Faker.Name.First());
                picklists.NonClinicalSpecialties.Add(Faker.Name.First());
                picklists.Departments.Add(Faker.Name.First());
                picklists.Credentials.Add(Faker.Name.First());
                
            }
            return picklists;
        }

    }
}
