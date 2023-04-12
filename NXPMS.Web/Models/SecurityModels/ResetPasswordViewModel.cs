using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SecurityModels
{
    public class ResetPasswordViewModel:BaseViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Display(Name = "Name")]
        public string UserFullName { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Confirm New Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
