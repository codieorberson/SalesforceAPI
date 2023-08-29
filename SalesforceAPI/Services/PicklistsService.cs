using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using RestSharp;
using SalesforceAPI.Helpers.Interfaces;
using SalesforceAPI.Models;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;

namespace SalesforceAPI.Services
{
    public class PicklistsService : IPicklistsService
    {
        ISalesforceAPIService salesforceAPIService;
        IJsonHelper jsonHelper;
        IMemoryCache memoryCache;
        ILogger logger;
        IConfiguration configuration;
        int cacheTimeout;

        public PicklistsService(ISalesforceAPIService salesforceAPIService, IJsonHelper jsonHelper, IMemoryCache memoryCache, ILogger logger, IConfiguration configuration)
        {
            this.salesforceAPIService = salesforceAPIService;
            this.jsonHelper = jsonHelper;
            this.memoryCache = memoryCache;
            this.logger = logger;
            this.configuration = configuration;
            cacheTimeout = configuration.GetValue<int>("Salesforce:PicklistsCacheTimeout");
        }

        public PicklistFields GetAllPicklists(UserType userType)
        {
            try
            {
                PicklistFields picklists = memoryCache.Get<PicklistFields>($"{userType}-picklists");
                return picklists;
            }
            catch(Exception ex)
            {
                logger.Exception(ex);
                return new PicklistFields();
            }
        }


        public void RefreshPicklists(UserType userType)
        {
            try
            {
                logger.Information($"PicklistsService.RefreshPicklists: {userType}");
                string jsonPicklists = salesforceAPIService.GetPicklists(userType);
                PicklistFields picklists = jsonHelper.ConstructPicklistsFromJson(jsonPicklists, userType);
                memoryCache.Remove($"{userType}-picklists");
                memoryCache.Set($"{userType}-picklists", picklists, DateTime.Now.AddMilliseconds(cacheTimeout).AddMinutes(1));
            }
            catch (KeyNotFoundException keyEx)
            {
                logger.Exception(keyEx);
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
            }

        }
    }
}
