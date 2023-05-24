﻿using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class MoveToPreviousStepModel:BaseViewModel
    {
        [Required]
        public int ReviewHeaderID { get; set; }
        public int CurrentStageID { get; set; }
        [Display(Name="Move From: ")]
        public string CurrentStageDescription { get; set; }

        [Required]
        [Display(Name="Move To*: ")]
        public int NewStageID { get; set; }
        public string NewStageDescription { get; set; }
        public int AppraiseeID { get; set; }
        public int ReviewSessionID { get; set; }
    }
}
