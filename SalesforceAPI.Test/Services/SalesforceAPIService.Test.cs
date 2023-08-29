using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Models;
using SalesforceAPI.Services;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;
using SSOModels.Metis;

namespace SalesforceAPI.Test.Services
{
    [TestFixture]
    public class SalesforceAPIServiceTest
    {
        Mock<IAccessTokenService> accessTokenService;
        Mock<IOptions<UserTypeMapping>> userTypeMapping;
        IConfiguration configuration;
        Mock<ILogger> logger;

        Mock<IRequestFactory> requestFactory;
        Mock<IRestRequest> restRequest;
        Mock<IRestClient> restClient;
        Mock<IRestResponse> restResponse;

        ISalesforceAPIService salesforceAPIService;

        [SetUp]
        public void Setup()
        {
            var mapping = new UserTypeMapping
            {
                UserIds = new Dictionary<string, string>
                {
                    { "Member", "012E0000000PRZEIA4" },
                    { "Supplier", "0122S0000002V6SQAU" }
                }
            };
            userTypeMapping = new Mock<IOptions<UserTypeMapping>>();
            userTypeMapping.Setup(x => x.Value).Returns(mapping);
            accessTokenService = new Mock<IAccessTokenService>();
            
            restRequest = new Mock<IRestRequest>();
            requestFactory = new Mock<IRequestFactory>();
            restClient = new Mock<IRestClient>();
            restResponse = new Mock<IRestResponse>();
            restClient.Setup(c => c.Execute(It.IsAny<IRestRequest>())).Returns(restResponse.Object);
            requestFactory.Setup(c => c.CreateClient()).Returns(restClient.Object);
            requestFactory.Setup(c => c.CreateRequest()).Returns(restRequest.Object);
            logger = new Mock<ILogger>();
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.localhost.json").Build(); 
            logger = new Mock<ILogger>();
            salesforceAPIService = new SalesforceAPIService(accessTokenService.Object, requestFactory.Object, userTypeMapping.Object, configuration, logger.Object);
        }

        [Test]
        public void GetRecord_ShouldReturnJsonRecord_WhenSalesforceApiCallSuccesful()
        {

            string id = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            string orgUrl = configuration.GetValue<string>("Salesforce:OrgUrl");
            string resourceUrl = $"/services/data/v55.0/sobjects/{recordType}/{id}";
            string accessToken = Faker.RandomNumber.Next(99999).ToString();

            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);
            restClient.Setup(x => x.BaseUrl).Returns(new Uri(orgUrl));
            restRequest.Setup(x => x.Resource).Returns(resourceUrl);
            restRequest.Setup(x => x.Method).Returns(Method.GET);
            restResponse.Setup(x => x.IsSuccessful).Returns(true);
            Contact contact = new Contact()
            {
                Id = id,
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last()
            };

            string expected = JsonConvert.SerializeObject(contact);
            
            restResponse.Setup(x => x.Content).Returns(expected);

            string actual = salesforceAPIService.GetRecord(id, recordType);

            Assert.AreEqual(expected, actual);
            accessTokenService.VerifyAll();
            restRequest.Verify(x => x.AddHeader("Authorization", string.Format("Bearer {0}", accessToken)), Times.Once());
        }




        [TestCase("expectedId")]
        [TestCase("")]
        public void CreateRecord_ShouldReturnTrue_WhenSalesforceApiCallSuccesful(string expected)
        {

            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            string orgUrl = configuration.GetValue<string>("Salesforce:OrgUrl");
            string resourceUrl = $"/services/data/v55.0/sobjects/{recordType}";
            string accessToken = Faker.RandomNumber.Next(99999).ToString();

            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);
            restClient.Setup(x => x.BaseUrl).Returns(new Uri(orgUrl));
            restRequest.Setup(x => x.Resource).Returns(resourceUrl);
            restRequest.Setup(x => x.Method).Returns(Method.POST);
            restResponse.Setup(x => x.IsSuccessful).Returns(!string.IsNullOrEmpty(expected));

            JObject responseObject = new JObject();
            responseObject.Add("id", expected);
            restResponse.Setup(x => x.Content).Returns(responseObject.ToString());
            

            string actual = salesforceAPIService.CreateRecord(jsonMessage, recordType);

            Assert.AreEqual(expected, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
            restRequest.Verify(x => x.AddJsonBody(jsonMessage), Times.Once());
            restRequest.Verify(x => x.AddHeader("Authorization", string.Format("Bearer {0}", accessToken)), Times.Once());

        }

        [Test]
        public void CreateRecord_ShouldLogErrorWhenExceptionThrown()
        {

            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();

            string accessToken = Faker.RandomNumber.Next(99999).ToString();
            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);

            restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Throws(It.IsAny<Exception>());
            string actual = salesforceAPIService.CreateRecord(jsonMessage, recordType);

            Assert.AreEqual(string.Empty, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
        }

        [Test]
        public void CreateRecord_ShouldLogErrorWhenConstructRestRequestMessageExceptionThrown()
        {

            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            accessTokenService.Setup(x => x.GetAccessToken()).Throws(It.IsAny<Exception>());

            string actual = salesforceAPIService.CreateRecord(jsonMessage, recordType);
            Assert.AreEqual(string.Empty, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
            restClient.Verify(x => x.Execute(It.IsAny<IRestRequest>()), Times.Never());

        }

        [TestCase(true)]
        [TestCase(false)]
        public void UpdateRecord_ShouldReturnTrue_WhenSalesforceApiCallSuccesful(bool expected)
        {
            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            string recordId = Faker.RandomNumber.Next(99999).ToString();
            string orgUrl = configuration.GetValue<string>("Salesforce:OrgUrl");
            string requestUrl = $"/services/data/v55.0/sobjects/{recordType}/{recordId}?_HttpMethod=PATCH";
            string accessToken = Faker.RandomNumber.Next(99999).ToString();

            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);
            restClient.Setup(x => x.BaseUrl).Returns(new Uri(orgUrl));
            restRequest.Setup(x => x.Resource).Returns(requestUrl);
            restRequest.Setup(x => x.Method).Returns(Method.POST);
            restResponse.Setup(x => x.IsSuccessful).Returns(expected);

            bool actual = salesforceAPIService.UpdateRecord(jsonMessage, recordType, recordId);

            Assert.AreEqual(expected, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
            restRequest.Verify(x => x.AddJsonBody(jsonMessage), Times.Once());
            restRequest.Verify(x => x.AddHeader("Authorization", string.Format("Bearer {0}", accessToken)), Times.Once());

        }

        [Test]
        public void UpdateRecord_ShouldLogErrorWhenExceptionThrown()
        {

            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            string recordId = Faker.RandomNumber.Next(99999).ToString();

            string accessToken = Faker.RandomNumber.Next(99999).ToString();
            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);

            restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Throws(It.IsAny<Exception>());

            bool actual = salesforceAPIService.UpdateRecord(jsonMessage, recordType, recordId);

            Assert.IsFalse(actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());

        }

        [Test]
        public void UpdateRecord_ShouldLogErrorWhenConstructRestRequestMessageExceptionThrown()
        {

            string jsonMessage = Faker.RandomNumber.Next(99999).ToString();
            string recordType = Faker.Name.First();
            string recordId = Faker.RandomNumber.Next(99999).ToString();

            accessTokenService.Setup(x => x.GetAccessToken()).Throws(It.IsAny<Exception>());

            bool actual = salesforceAPIService.UpdateRecord(jsonMessage, recordType, recordId);

            Assert.IsFalse(actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
            restClient.Verify(x => x.Execute(It.IsAny<IRestRequest>()), Times.Never());
        }

        [TestCase("BlahBlahBlahBlahBlah")]
        [TestCase("")]
        [TestCase(null)]
        public void GetPicklists_ShouldReturnExpectedString_WhenSalesforceApiCallSuccesful(string expected)
        {
            UserType userType = UserType.Member;
            string recordTypeId = Faker.Name.First();
            string orgUrl = configuration.GetValue<string>("Salesforce:OrgUrl");
            string requestUrl = $"/services/data/v55.0/ui-api/object-info/Contact/picklist-values/{recordTypeId}";
            string accessToken = Faker.RandomNumber.Next(99999).ToString();

            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);
            restClient.Setup(x => x.BaseUrl).Returns(new Uri(orgUrl));
            restRequest.Setup(x => x.Resource).Returns(requestUrl);
            restRequest.Setup(x => x.Method).Returns(Method.GET);
            restResponse.Setup(x => x.Content).Returns(expected);

            string actual = salesforceAPIService.GetPicklists(userType);

            Assert.AreEqual(expected, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());
            restRequest.Verify(x => x.AddHeader("Authorization", string.Format("Bearer {0}", accessToken)), Times.Once());

        }

        [Test]
        public void GetPicklists_ShouldLogErrorWhenExceptionThrown()
        {
            UserType userType = UserType.Member;

            string accessToken = Faker.RandomNumber.Next(99999).ToString();
            accessTokenService.Setup(x => x.GetAccessToken()).Returns(accessToken);

            restClient.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Throws(It.IsAny<Exception>());

            string actual = salesforceAPIService.GetPicklists(userType);

            Assert.AreEqual(string.Empty, actual);
            accessTokenService.Verify(x => x.GetAccessToken(), Times.Once());

        }
    }
}
