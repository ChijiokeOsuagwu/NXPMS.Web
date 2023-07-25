using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AddRecommendationViewModel:BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int RecommenderID { get; set; }

        [Display(Name="Recommended By:")]
        public string RecommenderName { get; set; }

        [Required]
        [Display(Name="Recommended As:")]
        public string RecommenderRole { get; set; }

        [Display(Name = "Recommended As:")]
        public string RecommenderRoleDescription { get; set; }

        [Required]
        [Display(Name = "Recommended For:")]
        [MaxLength(100)]
        public string RecommendedAction { get; set; }

        [Required]
        [Display(Name = "Justification")]
        [MaxLength(10000)]
        public string Remarks { get; set; }
    }
}
