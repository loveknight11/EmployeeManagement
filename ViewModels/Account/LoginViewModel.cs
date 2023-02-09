using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace EmployeeManagement.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remeber Me")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public List<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
