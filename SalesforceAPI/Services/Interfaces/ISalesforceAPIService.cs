using SalesforceAPI.Models;

namespace SalesforceAPI.Services.Interfaces
{
    public interface ISalesforceAPIService
    {
        string CreateRecord(string jsonMessage, string recordType);
        bool UpdateRecord(string jsonMessage, string recordType, string recordId);
        string GetPicklists(UserType userType);
        string GetRecord(string id, string recordType);
    }
}