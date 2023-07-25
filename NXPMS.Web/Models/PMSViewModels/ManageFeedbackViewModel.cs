using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ManageFeedbackViewModel:BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }

        [Required]
        [Display(Name="Problem(s) Encountered")]
        [MaxLength(5000)]
        public string ProblemDescription { get; set; }

        [Required]
        [Display(Name="Recommendations")]
        [MaxLength(5000)]
        public string SolutionDescription { get; set; }
    }
}
