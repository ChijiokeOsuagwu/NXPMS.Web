﻿using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ManageGradeProfileViewModel:BaseViewModel
    {
        public int? GradeHeaderId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name="Profile Name")]
        public string GradeHeaderName { get; set; }

        [MaxLength(250)]
        [Display(Name = "Profile Description")]
        public string GradeHeaderDescription { get; set; }
    }
}
