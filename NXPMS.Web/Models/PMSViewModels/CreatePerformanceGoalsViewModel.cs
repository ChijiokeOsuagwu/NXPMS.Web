using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class CreatePerformanceGoalsViewModel:BaseViewModel
    {
        public int? ReviewHeaderID { get; set; }

        [Required]
        public int ReviewSessionID { get; set; }
        public string ReviewSessionName { get; set; }

        [Required]
        public int AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }

        [Required]
        [Display(Name = "Performance Goal(s)")]
        public string PerformanceGoals { get; set; }
    }
}
