using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class CopyAppraisalGradesViewModel:BaseViewModel
    {
        public int ReviewSessionId { get; set; }

        [Required]
        [Display(Name="Copy From:")]
        public int GradeProfileId { get; set; }
    }
}
