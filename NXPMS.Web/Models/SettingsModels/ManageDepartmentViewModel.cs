using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SettingsModels
{
    public class ManageDepartmentViewModel:BaseViewModel
    {
        [Required]
        [MaxLength(5)]
        [Display(Name="Code")]
        public string DepartmentCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string DepartmentName { get; set; }
        public int? DepartmentHeadID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Department Head")]
        public string DepartmentHeadName { get; set; }
        public int? DepartmentAltHeadID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Alt. Department Head")]
        public string DepartmentAltHeadName { get; set; }

        public bool IsUpdate { get; set; }
        public string OldCode { get; set; }
    }
}
