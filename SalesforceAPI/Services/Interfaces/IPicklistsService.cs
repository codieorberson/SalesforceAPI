using SalesforceAPI.Models;
using System.Collections.Generic;

namespace SalesforceAPI.Services.Interfaces
{
    public interface IPicklistsService
    {
        PicklistFields GetAllPicklists(UserType userType);
        void RefreshPicklists(UserType userType);

    }
}