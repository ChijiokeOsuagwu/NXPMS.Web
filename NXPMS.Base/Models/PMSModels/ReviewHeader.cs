using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewHeader
    {
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public int ReviewStageId { get; set; }
        public string ReviewStageDescription { get; set; }
        public string ReviewStageActionDescription { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public int? PrimaryAppraiserId { get; set; }
        public string PrimaryAppraiserName { get; set; }
        public string FeedbackProblems { get; set; }
        public string FeedbackSolutions { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public string LineManagerComments { get; set; }
        public int? LineManagerRecommendation { get; set; }
        public string LineManagerRecommendationDescription { get; set; }
        public string UnitHeadComments { get; set; }
        public int? UnitHeadRecommendation { get; set; }
        public string UnitHeadRecommendationDescription { get; set; }
        public string DepartmentHeadComments { get; set; }
        public int? DepartmentHeadRecommendation { get; set; }
        public string DepartmentHeadRecommendationDescription { get; set; }
        public string HrComments { get; set; }
        public int? HrRecommendation { get; set; }
        public string HrRecommendationDescription { get; set; }
        public string ManagementComments { get; set; }
        public int? ManagementDecision { get; set; }
        public string ManagementDecisionDescription { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? TimeAccepted { get; set; }
        public bool IsFlagged { get; set; }
        public string FlaggedBy { get; set; }
        public DateTime? FlaggedTime { get; set; }
        public string PerformanceGoal { get; set; }
    }
}
