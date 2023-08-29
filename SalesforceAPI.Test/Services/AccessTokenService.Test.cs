using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Models;
using SalesforceAPI.Services;
using SSOLogging;

namespace SalesforceAPI.Test.Services
{
    [TestFixture]

    public class AccessTokenServiceTest
    {
        IMemoryCache memoryCache;

        Mock<IRequestFactory> requestFactory;
        Mock<IRestRequest> restRequest;
        Mock<IRestClient> restClient;
        Mock<IRestResponse> restResponse;


        IConfiguration configuration;
        Mock<ILogger> logger;
        AccessTokenService accessTokenService;

        [SetUp]
        public void Setup()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
            restRequest = new Mock<IRestRequest>();
            requestFactory = new Mock<IRequestFactory>();

            restClient = new Mock<IRestClient>();
            restResponse = new Mock<IRestResponse>();
            restClient.Setup(c => c.Execute(It.IsAny<IRestRequest>())).Returns(restResponse.Object);
            requestFactory.Setup(c => c.CreateClient()).Returns(restClient.Object);
            requestFactory.Setup(c => c.CreateRequest()).Returns(restRequest.Object);

            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.localhost.json").Build();
            logger = new Mock<ILogger>();


            accessTokenService = new AccessTokenService(memoryCache, requestFactory.Object, configuration, logger.Object);
        }

        [Test]
        public void GetAccessToken_ShouldReturnExistingToken_WhenCacheExists()
        {
            string tokenValue = Faker.RandomNumber.Next(0, 999999).ToString();
            memoryCache.Set("salesforce-access-token", tokenValue, DateTime.Now.AddMinutes(configuration.GetValue<int>("Salesforce:TokenCacheTimeout")));

            string expected = (string)memoryCache.Get("salesforce-access-token");

            string actual = accessTokenService.GetAccessToken();

            Assert.AreEqual(expected, actual);
            restRequest.Verify(x => x.AddQueryParameter(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            restClient.Verify(x => x.Execute(It.IsAny<RestRequest>()), Times.Never);
        }

        [Test]
        public void GetAccessToken_ShouldReturnNewToken_WhenCacheEmpty()
        {
            TokenResponse tokenResponse = new TokenResponse()
            {
                access_token = Faker.RandomNumber.Next(0, 999999).ToString()
            };

            restResponse.Setup(x => x.Content).Returns(JsonConvert.SerializeObject(tokenResponse));


            string actual = accessTokenService.GetAccessToken();

            Assert.AreEqual(tokenResponse.access_token, actual);
            restRequest.Verify(x => x.AddQueryParameter("grant_type", "password"), Times.Once);
            restRequest.Verify(x => x.AddQueryParameter("client_id", configuration.GetValue<string>("Salesforce:ClientId")), Times.Once);
            restRequest.Verify(x => x.AddQueryParameter("client_secret", configuration.GetValue<string>("Salesforce:ClientSecret")), Times.Once);
            restRequest.Verify(x => x.AddQueryParameter("username", configuration.GetValue<string>("Salesforce:Username")), Times.Once);
            restRequest.Verify(x => x.AddQueryParameter("password", configuration.GetValue<string>("Salesforce:Password")), Times.Once);
            Assert.AreEqual((string)memoryCache.Get("salesforce-access-token"), actual);

        }

        [Test]
        public void GetAccessToken_ShouldLogError_WhenHttpRequestExceptionThrown()
        {
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.bad.json").Build();
            accessTokenService = new AccessTokenService(memoryCache, requestFactory.Object, configuration, logger.Object);

            string expected = string.Empty;

            string actual = accessTokenService.GetAccessToken();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAccessToken_ShouldLogError_WhenCacheExceptionThrown()
        {

            string expected = string.Empty;
            accessTokenService = new AccessTokenService(null, requestFactory.Object, configuration, logger.Object);

            TokenResponse tokenResponse = new TokenResponse()
            {
                access_token = null
            };

            restResponse.Setup(x => x.Content).Returns(JsonConvert.SerializeObject(tokenResponse));


            string actual = accessTokenService.GetAccessToken();

            Assert.AreEqual(expected, actual);


        }
    }
}
