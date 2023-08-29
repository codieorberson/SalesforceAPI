using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SalesforceAPI.Helpers.Interfaces;
using SalesforceAPI.Models;
using SSOLogging;

namespace SalesforceAPI.Helpers
{
    public class JsonHelper : IJsonHelper
    {
        ILogger logger;
        public JsonHelper(ILogger logger)
        {
            this.logger = logger;
        }
        public PicklistFields ConstructPicklistsFromJson(string jsonPicklists, UserType userType)
        {
            try
            {
                var defaultValue = new List<string>();
                var picklistResponse = JsonConvert.DeserializeObject<PicklistResponse>(jsonPicklists);
                var picklists = new PicklistFields
                {
                    ClinicalSpecialties = userType == UserType.Member ? GetValueList(picklistResponse.PicklistFieldValues, "Clinical_Specialties__c") : defaultValue,
                    NonClinicalSpecialties = userType == UserType.Member ? GetValueList(picklistResponse.PicklistFieldValues, "Non_Clinical_Specialties__c") : defaultValue,
                    Credentials = GetValueList(picklistResponse.PicklistFieldValues, "ContactCredentials__c"),
                    Departments = GetValueList(picklistResponse.PicklistFieldValues, "Department_s__c"),
                    Roles = GetValueList(picklistResponse.PicklistFieldValues, "Role_s__c"),
                    Salutations = GetValueList(picklistResponse.PicklistFieldValues, "Salutation")
                };
                return picklists;
            }
            catch(Exception ex)
            {
                logger.Exception(ex);
                throw;
            }
        }

        private List<string> GetValueList(Dictionary<string, PicklistField> picklistFieldValues, string key)
        {
            if (picklistFieldValues.TryGetValue(key, out var picklistField))
            {
                return picklistField.Values.Select(v => v.Value).ToList();
            }
            return new List<string>();
        }
    }

    public class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss.fffffff";

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Format));
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
