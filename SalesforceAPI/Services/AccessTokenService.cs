using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Models;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;
using System;

namespace SalesforceAPI.Services
{
    public class AccessTokenService : IAccessTokenService
    {
        private int cacheTimeout;
        private string loginUrl;
        private IMemoryCache memoryCache;
        private IRequestFactory requestFactory;
        private IConfiguration configuration;
        private ILogger logger;

        public AccessTokenService(IMemoryCache memoryCache, IRequestFactory requestFactory, IConfiguration configuration, ILogger logger)
        {
            this.memoryCache = memoryCache;
            this.requestFactory = requestFactory;
            this.configuration = configuration;
            this.logger = logger;
            cacheTimeout = configuration.GetValue<int>("Salesforce:TokenCacheTimeout");
            loginUrl = configuration.GetValue<string>("Salesforce:LoginUrl");
        }
        public string GetAccessToken()
        {
            try
            {
                string accessToken = GetAccessTokenFromCache();
                if (!String.IsNullOrEmpty(accessToken)) return accessToken;
                accessToken = GetAccessTokenFromSalesforce();
                if (string.IsNullOrEmpty(accessToken)) return string.Empty;
                AddAccessTokenToCache(accessToken);
                return accessToken;

            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }


        }

        private string GetAccessTokenFromCache()
        {
            try
            {
                return (string)memoryCache.Get("salesforce-access-token");
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }

        }

        private void AddAccessTokenToCache(string accessToken)
        {
            try
            {
                memoryCache.Set("salesforce-access-token", accessToken, DateTime.Now.AddMinutes(cacheTimeout));
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
            }

        }

        private string GetAccessTokenFromSalesforce()
        {
            try
            {
                IRestRequest restRequest = requestFactory.CreateRequest();
                IRestClient restClient = requestFactory.CreateClient();

                restRequest.AddQueryParameter("grant_type", "password");
                restRequest.AddQueryParameter("client_id", configuration.GetValue<string>("Salesforce:ClientId"));
                restRequest.AddQueryParameter("client_secret", configuration.GetValue<string>("Salesforce:ClientSecret"));
                restRequest.AddQueryParameter("username", configuration.GetValue<string>("Salesforce:Username"));
                restRequest.AddQueryParameter("password", configuration.GetValue<string>("Salesforce:Password"));

                restClient.BaseUrl = new Uri(loginUrl);
                restRequest.Method = Method.POST;

                var restResponse = restClient.Execute(restRequest);

                return (JsonConvert.DeserializeObject<TokenResponse>(restResponse.Content).access_token);

            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }

        }
    }
}
