using SalesforceAPI.Models;
using System.Collections.Generic;

namespace SalesforceAPI.Helpers.Interfaces
{
    public interface IJsonHelper
    {
        PicklistFields ConstructPicklistsFromJson(string jsonPicklists, UserType userType);
    }
}