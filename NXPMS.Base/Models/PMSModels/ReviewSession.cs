using NXPMS.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewSession
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public int ReviewTypeId { get; set; }
        public string ReviewTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int MinNoOfCompetencies { get; set; }
        public int MaxNoOfCompetencies { get; set; }
        public decimal TotalCompetencyScore { get; set; }
        public decimal TotalKpaScore { get; set; }
        public decimal TotalCombinedScore { get; set; }
        public bool IsActive { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
