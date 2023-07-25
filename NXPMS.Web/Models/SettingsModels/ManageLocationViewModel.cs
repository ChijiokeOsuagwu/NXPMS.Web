using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SettingsModels
{
    public class ManageLocationViewModel:BaseViewModel
    {

        public int LocationID { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Name")]
        public string LocationName { get; set; }

        [MaxLength(100)]
        [Display(Name = "State/Province")]
        public string LocationState { get; set; }

        [MaxLength(100)]
        [Display(Name = "Country")]
        public string LocationCountry { get; set; }


        [Display(Name = "Location Head")]
        public int? LocationHeadID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Location Head")]
        public string LocationHeadName { get; set; }

        [Display(Name = "Alt. Location Head")]
        public int? LocationAltHeadID { get; set; }

        [MaxLength(100)]
        [Display(Name = "Alt. Location Head")]
        public string LocationAltHeadName { get; set; }

        public bool IsUpdate { get; set; }
    }
}
