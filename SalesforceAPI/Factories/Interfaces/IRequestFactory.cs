using RestSharp;

namespace SalesforceAPI.Factories.Interfaces
{
    public interface IRequestFactory
    {
        IRestClient CreateClient();

        IRestRequest CreateRequest();
    }
}