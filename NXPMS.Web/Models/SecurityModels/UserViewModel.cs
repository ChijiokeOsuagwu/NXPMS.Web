using NXPMS.Base.Models.SecurityModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SecurityModels
{
    public class UserViewModel:BaseViewModel
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(60)]
        public string Username { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public int EmployeeId { get; set; }

        [Display(Name="Lock Account")]
        public bool LockoutEnabled { get; set; }

        [Display(Name="End Lock On")]
        public DateTime? LockoutEnd { get; set; }

        [Display(Name = "End Lock On")]
        public string LockoutEndFormatted { get; set; }

        [Required]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; } = DateTime.UtcNow;

        public ApplicationUser ConvertToUser()
        {
            return new ApplicationUser
            {
                CreatedBy = CreatedBy,
                CreatedTime = CreatedTime,
                EmployeeId = EmployeeId,
                FullName = FullName,
                Id = UserId,
                LockoutEnabled = LockoutEnabled,
                LockoutEnd = LockoutEnd,
                ModifiedBy = ModifiedBy,
                ModifiedTime = ModifiedTime,
                Username = Username,
            };
        }

        public UserViewModel ExtractFromUser(ApplicationUser user)
        {
            return new UserViewModel
            {
                CreatedBy = user.CreatedBy,
                CreatedTime = user.CreatedTime,
                EmployeeId = user.EmployeeId,
                FullName = user.FullName,
                UserId = user.Id,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                ModifiedBy = user.ModifiedBy,
                ModifiedTime = user.ModifiedTime,
                Username = user.Username,
            };
        }

    }
}
