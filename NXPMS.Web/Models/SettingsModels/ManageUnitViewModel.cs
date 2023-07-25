using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SettingsModels
{
    public class ManageUnitViewModel:BaseViewModel
    {
        [Required]
        [MaxLength(5)]
        [Display(Name = "Code")]
        public string UnitCode { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string UnitName { get; set; }
        public int? UnitHeadID { get; set; }

        [MaxLength(150)]
        [Display(Name = "Unit Head")]
        public string UnitHeadName { get; set; }
        public int? UnitAltHeadID { get; set; }

        [MaxLength(150)]
        [Display(Name = "Alt. Unit Head")]
        public string UnitAltHeadName { get; set; }

        [Display(Name = "Department")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        public bool IsUpdate { get; set; }
        public string OldCode { get; set; }

    }
}
