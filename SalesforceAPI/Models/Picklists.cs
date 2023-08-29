using System;
using System.Collections.Generic;
using System.Text;

namespace SalesforceAPI.Models
{
    public class PicklistFields
    {
        public IList<string> Salutations { get; set; }
        public IList<string> ClinicalSpecialties { get; set; }
        public IList<string> NonClinicalSpecialties { get; set; }
        public IList<string> Departments { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Credentials { get; set; }
    }

    public class PicklistValue
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class PicklistField
    {
        public IList<PicklistValue> Values { get; set; }
    }

    public class PicklistResponse
    {
        public string ETag { get; set; }
        public Dictionary<string, PicklistField> PicklistFieldValues { get; set; }
    }

    

}
