using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SecurityModels
{
    public class LoginViewModel:BaseViewModel
    {
        [Required]
        [Display(Name="Username")]
        [MaxLength(50, ErrorMessage = "Username must not exceed 50 characters")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
        public string ReturnUrl { get; set; }
    }
}
