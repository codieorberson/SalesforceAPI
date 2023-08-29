using SalesforceAPI.Models;
using SSOModels.Metis;
using System;

namespace SalesforceAPI.Services.Interfaces
{
    public interface IContactCRUDService
    {
        string CreateContact(Contact contact);
        string CreateContact(ContactCRUD contact);
        bool UpdateContact(Contact contact);
        bool UpdateContact(ContactCRUD contact);
        ContactCRUD GetContact(string salesforceId);




    }
}