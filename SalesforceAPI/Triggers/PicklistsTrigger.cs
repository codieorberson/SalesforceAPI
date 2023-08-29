using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Threading;
using SalesforceAPI.Services.Interfaces;
using SalesforceAPI.Models;
using SSOLogging;
using Microsoft.Extensions.Configuration;
using Serilog.Context;

namespace SalesforceAPI.Triggers
{
    public class PicklistsTrigger : IHostedService
    {
        IPicklistsService picklistsService;
        ILogger logger;
        int delay;
        bool enabled;

        public PicklistsTrigger(IPicklistsService picklistsService, IConfiguration configuration, ILogger logger)
        {
            this.picklistsService = picklistsService;
            this.logger = logger;

            enabled = configuration.GetValue<bool>("Salesforce:PicklistsTriggerEnabled");
            delay = configuration.GetValue<int>("Salesforce:PicklistsCacheTimeout");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (enabled) StartTimer();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async void StartTimer()
        {
            try
            {
                await Task.Delay(2500);
                while (true)
                {
                    using (LogContext.PushProperty("CorrelationId", Guid.NewGuid().ToString()))
                    {
                        _ = Task.Run(() =>
                        {
                            //Save for Supplier Registration
                            //foreach (UserType userType in Enum.GetValues(typeof(UserType)))
                            //{
                            //    picklistsService.RefreshPicklists(userType);
                            //}

                            picklistsService.RefreshPicklists(UserType.Member);

                        });
                    }
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
            }
        }
    }
}
