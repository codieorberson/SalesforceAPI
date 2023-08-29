using RestSharp;
using SalesforceAPI.Factories.Interfaces;

namespace SalesforceAPI.Factories
{
    public class RequestFactory : IRequestFactory
    {
        public IRestClient CreateClient()
        {
            return new RestClient();
        }

        public IRestRequest CreateRequest()
        {
            return new RestRequest();
        }
    }
}
