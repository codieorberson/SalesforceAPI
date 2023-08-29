using SSOLogging;
using Microsoft.Extensions.Configuration;
using SalesforceAPI.Services.Interfaces;
using Moq;
using NUnit.Framework;
using SalesforceAPI.Triggers;
using SalesforceAPI.Models;

namespace SalesforceAPI.Test.Triggers
{
    [TestFixture]
    public class PicklistsTriggerTest
    {
        Mock<IPicklistsService> picklistsService;
        IConfiguration configuration;
        Mock<ILogger> logger;
        PicklistsTrigger picklistsTrigger;

        [SetUp]
        public void Setup()
        {
            picklistsService = new Mock<IPicklistsService>();
            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.localhost.json").Build();
            logger = new Mock<ILogger>();
            picklistsTrigger = new PicklistsTrigger(picklistsService.Object, configuration, logger.Object);
        }

        [Test]
        public async Task StartTimer_ExecutesRefreshPicklistsInstantly_AndReoccuringly()
        {
            await Task.Delay(2000);

            picklistsService.Setup(x => x.RefreshPicklists(It.IsAny<UserType>())).Verifiable();
            
            await picklistsTrigger.StartAsync(CancellationToken.None);
            await Task.Delay(3000);

            picklistsService.Verify(x => x.RefreshPicklists(It.IsAny<UserType>()), Times.AtLeast(1));
        }

        [Test]
        public void StartTimer_ShouldNotCallService_WhenExceptionThrown()
        {
            logger.Setup(x => x.Information(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Throws(It.IsAny<Exception>());

            picklistsTrigger.StartAsync(CancellationToken.None);

            picklistsService.Verify(x => x.RefreshPicklists(It.IsAny<UserType>()), Times.Never);
        }
    }
}
