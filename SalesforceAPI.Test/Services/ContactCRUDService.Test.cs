using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SalesforceAPI.Models;
using SalesforceAPI.Services;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;
using SSOModels.Metis;

namespace SalesforceAPI.Test.Services
{
    [TestFixture]
    public class ContactCRUDServiceTest
    {
        IContactCRUDService contactCRUDService;
        Mock<ISalesforceAPIService> salesforceAPIService;
        Mock<ILogger> logger;

        [SetUp]
        public void Setup()
        {
            salesforceAPIService = new Mock<ISalesforceAPIService>();
            logger = new Mock<ILogger>();
            contactCRUDService = new ContactCRUDService(salesforceAPIService.Object, logger.Object);
        }

        [Test]
        public void CreateContactWithContactModel_ShouldCreateJsonAndExecuteSalesforceCreateAPI()
        {
            string expected = Faker.RandomNumber.Next(1, 99999).ToString();
            string firstName = Faker.Name.First();
            string lastName = Faker.Name.Last();
            DateTime securityEndDate = DateTime.Now;

            ContactCRUD contactCrud = new ContactCRUD()
            {
                Id = expected,
                FirstName = firstName,
                LastName = lastName,
                Security_End_Dt__c = securityEndDate
            };

            string jsonContact = JsonConvert.SerializeObject(contactCrud);
            salesforceAPIService.Setup(x => x.CreateRecord(jsonContact, "Contact")).Returns(expected);

            string actual = contactCRUDService.CreateContact(contactCrud);

            Assert.AreEqual(expected, actual);
            salesforceAPIService.Verify(x => x.CreateRecord(jsonContact, "Contact"), Times.Once());
        }

        [Test]
        public void CreateContactWithContactModel_ShouldReturnEmptyString_WhenExceptionThrown()
        {
            salesforceAPIService.Setup(x => x.CreateRecord(It.IsAny<string>(), "Contact")).Throws(It.IsAny<Exception>());

            string actual = contactCRUDService.CreateContact(new Contact());

            Assert.AreEqual(string.Empty, actual);
            salesforceAPIService.Verify(x => x.CreateRecord(It.IsAny<string>(), "Contact"), Times.Once());
        }

        [Test]
        public void CreateContactWithContactCrudModel_ShouldCreateJsonAndExecuteSalesforceCreateAPI()
        {
            var expected = Faker.RandomNumber.Next(1, 99999).ToString();
            ContactCRUD contact = new ContactCRUD()
            {
                Id = expected,
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Security_End_Dt__c = DateTime.Now
            };
            string jsonContact = JsonConvert.SerializeObject(contact);
            salesforceAPIService.Setup(x => x.CreateRecord(jsonContact, It.IsAny<string>())).Returns(expected);

            string actual = contactCRUDService.CreateContact(contact);

            Assert.AreEqual(expected, actual);
            salesforceAPIService.Verify(x => x.CreateRecord(jsonContact, "Contact"), Times.Once());
        }

        [Test]
        public void CreateContactWithContactCrudModel_ShouldReturnFalse_WhenExceptionThrown()
        {
            salesforceAPIService.Setup(x => x.CreateRecord(It.IsAny<string>(), "Contact")).Throws(It.IsAny<Exception>());

            string actual = contactCRUDService.CreateContact(new ContactCRUD());

            Assert.AreEqual(string.Empty, actual);
            salesforceAPIService.Verify(x => x.CreateRecord(It.IsAny<string>(), "Contact"), Times.Once());
        }

        [Test]
        public void UpdateContactWithContactModel_ShouldCreateJsonAndExecuteSalesforceUpdateAPI()
        {
            string id = Faker.RandomNumber.Next(1, 99999).ToString();
            string firstName = Faker.Name.First();
            string lastName = Faker.Name.Last();
            DateTime securityEndDate = DateTime.Now;
            Contact contact = new Contact()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Security_End_Dt__c = securityEndDate,
                Admin_Console__c = Faker.Lorem.Sentence()
            };

            ContactCRUD contactCrud = new ContactCRUD()
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Security_End_Dt__c = securityEndDate
            };
            string jsonContact = JsonConvert.SerializeObject(contactCrud);
            salesforceAPIService.Setup(x => x.UpdateRecord(jsonContact, "Contact", contactCrud.Id)).Returns(true);

            bool actual = contactCRUDService.UpdateContact(contact);

            Assert.IsTrue(actual);
            salesforceAPIService.Verify(x => x.UpdateRecord(jsonContact, "Contact", contactCrud.Id), Times.Once());
        }

        [Test]
        public void UpdateContactWithContactModel_ShouldReturnFalse_WhenExceptionThrown()
        {
            salesforceAPIService.Setup(x => x.UpdateRecord(It.IsAny<string>(), "Contact", It.IsAny<string>())).Throws(It.IsAny<Exception>());

            bool actual = contactCRUDService.UpdateContact(new Contact());

            Assert.IsFalse(actual);
            salesforceAPIService.Verify(x => x.UpdateRecord(It.IsAny<string>(), "Contact", It.IsAny<string>()), Times.Once());
        }

        
        [Test]
        public void UpdateContactWithContactCrudModel_ShouldCreateJsonAndExecuteSalesforceUpdateAPI()
        {
            ContactCRUD contact = new ContactCRUD()
            {
                Id = Faker.RandomNumber.Next(1, 99999).ToString(),
                FirstName = Faker.Name.First(),
                LastName = Faker.Name.Last(),
                Security_End_Dt__c = DateTime.Now
            };
            string jsonContact = JsonConvert.SerializeObject(contact);
            salesforceAPIService.Setup(x => x.UpdateRecord(jsonContact, "Contact", contact.Id)).Returns(true);

            bool actual = contactCRUDService.UpdateContact(contact);

            Assert.IsTrue(actual);
            salesforceAPIService.Verify(x => x.UpdateRecord(jsonContact, "Contact", contact.Id), Times.Once());
        }

        

        [Test]
        public void UpdateContactWithContactCrudModel_ShouldReturnFalse_WhenExceptionThrown()
        {
            salesforceAPIService.Setup(x => x.UpdateRecord(It.IsAny<string>(), "Contact", It.IsAny<string>())).Throws(It.IsAny<Exception>());

            bool actual = contactCRUDService.UpdateContact(new ContactCRUD());

            Assert.IsFalse(actual);
            salesforceAPIService.Verify(x => x.UpdateRecord(It.IsAny<string>(), "Contact", It.IsAny<string>()), Times.Once());
        }
    }
}
