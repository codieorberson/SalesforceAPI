using System;
using System.Collections.Generic;
using System.Text;

namespace SalesforceAPI.Models
{
    public enum UserType
    {
        Member,
        Supplier
    }

    public class UserTypeMapping
    {
        public Dictionary<string, string> UserIds { get; set; }
    }
}
