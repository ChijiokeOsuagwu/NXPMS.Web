﻿using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class EvaluationResultViewModel:BaseViewModel
    {
        public int ReviewSessionID { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearID { get; set; }
        public string ReviewYearName { get; set; }
        public int ReviewHeaderID { get; set; }
        public int AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }

        public int AppraiserID { get; set; }
        public string AppraiserName { get; set; }
        public int AppraiserRoleID { get; set; }
        public string AppraiserRoleName { get; set; }
        public string AppraiserTypeDescription { get; set; }
        public string AppraisalTime { get; set; }

        public string PerformanceRating { get; set; }
        public string PerformanceRank { get; set; }
        public decimal TotalScoreObtained { get; set; }

        public decimal QuantitativeScoreObtained { get; set; }

        public decimal QualitativeScoreObtained { get; set; }

        public decimal QuantitativeScoreObtainable { get; set; }

        public decimal QualitativeScoreObtainable { get; set; }
        public decimal TotalScoreObtainable { get; set; }
    }
}
