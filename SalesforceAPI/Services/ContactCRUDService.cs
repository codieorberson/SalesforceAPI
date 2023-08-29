using AutoMapper;
using Newtonsoft.Json;
using SalesforceAPI.Models;
using SalesforceAPI.Services.Interfaces;
using SSOLogging;
using SSOModels.Metis;
using System;

namespace SalesforceAPI.Services
{
    public class ContactCRUDService : IContactCRUDService
    {
        ISalesforceAPIService salesforceAPIService;
        ILogger logger;
        private Mapper mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<Contact, ContactCRUD>()));
        public ContactCRUDService(ISalesforceAPIService salesforceAPIService, ILogger logger)
        {
            this.salesforceAPIService = salesforceAPIService;
            this.logger = logger;
        }
        public ContactCRUD GetContact(string salesforceId)
        {
            try
            {
                string jsonContact = salesforceAPIService.GetRecord(salesforceId, "Contact");
                return JsonConvert.DeserializeObject<ContactCRUD>(jsonContact);
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return new ContactCRUD();
            }
        }

        public string CreateContact(Contact contact)
        {
            try
            {
                var crudContact = ConvertContactModel(contact);
                string jsonContact = JsonConvert.SerializeObject(crudContact);
                return salesforceAPIService.CreateRecord(jsonContact, "Contact");
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }
        }
        public string CreateContact(ContactCRUD contact)
        {
            try
            {
                string jsonContact = JsonConvert.SerializeObject(contact);
                return salesforceAPIService.CreateRecord(jsonContact, "Contact");
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return string.Empty;
            }
        }

        public bool UpdateContact(Contact contact)
        {
            try
            {
                var crudContact = ConvertContactModel(contact);
                string jsonContact = JsonConvert.SerializeObject(crudContact);
                return salesforceAPIService.UpdateRecord(jsonContact, "Contact", crudContact.Id);
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return false;
            }
        }

        public bool UpdateContact(ContactCRUD contact)
        {
            try
            {
                string jsonContact = JsonConvert.SerializeObject(contact);
                return salesforceAPIService.UpdateRecord(jsonContact, "Contact", contact.Id);
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return false;
            }

        }
        private ContactCRUD ConvertContactModel(Contact contact)
        {
            try
            {
                var crudContact = mapper.Map<ContactCRUD>(contact);
                return crudContact;
            }
            catch (Exception ex)
            {
                logger.Exception(ex);
                return null;
            }

        }
    }
}
