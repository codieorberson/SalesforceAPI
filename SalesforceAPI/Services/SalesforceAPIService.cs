using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Models;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;
using System;

namespace SalesforceAPI.Services
{
    public class SalesforceAPIService : ISalesforceAPIService
    {
        IAccessTokenService accessTokenService;
        IRequestFactory requestFactory;
        UserTypeMapping userTypeMapping;
        ILogger logger;
        string orgUrl;
        string apiPath;

        public SalesforceAPIService(IAccessTokenService accessTokenService, IRequestFactory requestFactory, IOptions<UserTypeMapping> userTypeMapping, IConfiguration configuration, ILogger logger)
        {
            this.accessTokenService = accessTokenService;
            this.requestFactory = requestFactory;
            this.userTypeMapping = userTypeMapping.Value;
            this.logger = logger;
            this.orgUrl = configuration.GetValue<string>("Salesforce:OrgUrl");
            this.apiPath = configuration.GetValue<string>("Salesforce:ApiPath");
        }
        public string CreateRecord(string jsonMessage, string recordType)
        {
            try
            {
                IRestRequest restRequest = requestFactory.CreateRequest();
                IRestClient restClient = requestFactory.CreateClient();

                string resource = $"{apiPath}/sobjects/{recordType}";

                restClient.BaseUrl = new Uri(orgUrl);
                restRequest.Resource = resource;
                restRequest.Method = Method.POST;
                restRequest = ConstructRestRequestMessage(restRequest, jsonMessage);
                if (restRequest == null) return string.Empty;
                var restReponse = restClient.Execute(restRequest);
                if (!restReponse.IsSuccessful)
                {
                    throw new Exception(restReponse.ErrorMessage);
                };
                JObject responseContent = JObject.Parse(restReponse.Content);
                string id = responseContent["id"]?.ToString();
                return id;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }

        }

        public string GetRecord(string id, string recordType)
        {
            try
            {
                IRestRequest restRequest = requestFactory.CreateRequest();
                IRestClient restClient = requestFactory.CreateClient();

                string resource = $"{apiPath}/sobjects/{recordType}/{id}";

                restClient.BaseUrl = new Uri(orgUrl);
                restRequest.Resource = resource;
                restRequest.Method = Method.GET;

                restRequest = ConstructRestRequestMessage(restRequest);
                if (restRequest == null) return string.Empty;
                var restReponse = restClient.Execute(restRequest);
                if (!restReponse.IsSuccessful)
                {
                    return string.Empty;
                }
                return restReponse.Content;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }

        }


        public bool UpdateRecord(string jsonMessage, string recordType, string recordId)
        {
            try
            {
                IRestRequest restRequest = requestFactory.CreateRequest();
                IRestClient restClient = requestFactory.CreateClient();

                string resource = $"{apiPath}/sobjects/{recordType}/{recordId}?_HttpMethod=PATCH";

                restClient.BaseUrl = new Uri(orgUrl);
                restRequest.Resource = resource;
                restRequest.Method = Method.POST;

                restRequest = ConstructRestRequestMessage(restRequest, jsonMessage);
                if (restRequest == null) return false;
                var restReponse = restClient.Execute(restRequest);
                return restReponse.IsSuccessful;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return false;
            }

        }

        public string GetPicklists(UserType userType)
        {
            try
            {
                IRestRequest restRequest = requestFactory.CreateRequest();
                IRestClient restClient = requestFactory.CreateClient();

                string recordTypeId = userTypeMapping.UserIds[userType.ToString()];
                string resource = $"{apiPath}/ui-api/object-info/Contact/picklist-values/{recordTypeId}";

                restClient.BaseUrl = new Uri(orgUrl);
                restRequest.Resource = resource;
                restRequest.Method = Method.GET;
                restRequest = ConstructRestRequestMessage(restRequest);
                if (restRequest == null) return string.Empty;
                var restReponse = restClient.Execute(restRequest);

                return restReponse.Content;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }



        }
        private IRestRequest ConstructRestRequestMessage(IRestRequest restRequest, string jsonMessage = null)
        {
            try
            {
                string accessToken = accessTokenService.GetAccessToken();
                restRequest.AddHeader("Authorization", string.Format("Bearer {0}", accessToken));
                restRequest.AddHeader("Accept", "application/json");
                if(!string.IsNullOrEmpty(jsonMessage)) restRequest.AddJsonBody(jsonMessage);
                return restRequest;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return null;
            }

        }
    }
}
