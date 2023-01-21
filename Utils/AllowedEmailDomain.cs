using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utils
{
    public class AllowedEmailDomain : ValidationAttribute
    {
        private readonly string allowedDomain;

        public AllowedEmailDomain(string AllowedDomain)
        {
            allowedDomain = AllowedDomain;
        }
        public override bool IsValid(object value)
        {
            string[] st = value.ToString().Split('@');
            return st[1].ToLower() == allowedDomain.ToLower();
        }
    }
}
