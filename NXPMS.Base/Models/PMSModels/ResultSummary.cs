using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ResultSummary
    {
        public int ResultSummaryId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public int AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public int AppraiserId { get; set; }
        public string AppraiserName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public decimal KpaScoreTotal { get; set; }
        public decimal KpaScoreObtained { get; set; }
        public decimal CompetencyScoreTotal { get; set; }
        public decimal CompetencyScoreObtained { get; set; }
        public decimal CombinedScoreTotal { get; set; }
        public decimal CombinedScoreObtained { get; set; }
        public int ScoreRank { get; set; }
        public string ScoreRankDescription { get; set; }
        public string PerformanceRating { get; set; }
        public string AppraiserTypeDescription { get; set; }
        public string AppraiserRoleDescription { get; set; }
        public string EmployeeNo { get; set; }
        public string CurrentDesignation { get; set; }
        public bool IsMain { get; set; }
        
    }
}
