using Newtonsoft.Json;
using SalesforceAPI.Helpers;
using System;

namespace SalesforceAPI.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ContactCRUD
    {
        public string AccountId { get; set; }
        public bool? Active__c { get; set; }
        public string AssistantName { get; set; }
        public string AssistantPhone { get; set; }
        public string Asst_Email__c { get; set; }
        public string Clinical_Specialties__c { get; set; }
        public string Company_Name__c { get; set; }
        public string Con_Comment__c { get; set; }
        public string Contact_Verified_Source__c { get; set; }
        public string ContactCredentials__c { get; set; }
        public string Corrsp_Org__c { get; set; }
        public string Department_s__c { get; set; }
        public bool? DoNotCall { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? Dt_Time_to_UHC__c { get; set; }
        public string Email { get; set; }
        public bool? Emp_Flag__c { get; set; }
        public string Fax { get; set; }
        public string FirstName { get; set; }
        public string Gender__c { get; set; }
        public string Guid__c { get; set; }
        public bool? HasOptedOutOfEmail { get; set; }
        [JsonIgnore]
        public string Id { get; set; }
        public string Inact_By__c { get; set; }
        public string Interests__c { get; set; }
        public string IPlan_Con_Id__c { get; set; }
        public string LastName { get; set; }
        public string MailingCity { get; set; }
        public string MailingCountry { get; set; }
        public string MailingPostalCode { get; set; }
        public string MailingState { get; set; }
        public string MailingStreet { get; set; }
        public string Middle_Name__c { get; set; }
        public string MidName__c { get; set; }
        public string MobilePhone { get; set; }
        public string MP_Login_Status__c { get; set; }
        public bool? Never_Mail__c { get; set; }
        public bool? Never_UHC_News__c { get; set; }
        public string Non_Clinical_Specialties__c { get; set; }
        public string Okta_Username__c { get; set; }
        public string OtherCity { get; set; }
        public string OtherCountry { get; set; }
        public string OtherPostalCode { get; set; }
        public string OtherState { get; set; }
        public string OtherStreet { get; set; }
        public string Phone { get; set; }
        public string Prd_Req__c { get; set; }
        public string Pref_Comm_Meth__c { get; set; }
        public string PrefName__c { get; set; }
        public string Role_s__c { get; set; }
        public string Salutation { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? SAT_Disclm_Dt__c { get; set; }
        public string Sbl_Cont_Id__c { get; set; }
        public string Secndry_Job_Ttl__c { get; set; }
        public string Security_Comment__c { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? Security_End_Dt__c { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? Security_Strt_Dt__c { get; set; }
        public string Site_Type__c { get; set; }
        public string Src__c { get; set; }
        public string Status__c { get; set; }
        public string Suffix__c { get; set; }
        public string Title { get; set; }
        public string UHC_Login__c { get; set; }
        public string User_Name__c { get; set; }
    }
}
