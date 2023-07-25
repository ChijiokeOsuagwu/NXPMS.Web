using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ScoreSummary
    {
        public int ReviewHeaderId { get; set; }
        public int AppraiserId { get; set; }
        public decimal QuantitativeScore { get; set; }
        public decimal QualitativeScore { get; set; }
        public decimal TotalPerformanceScore { get; set; }
    }
}
