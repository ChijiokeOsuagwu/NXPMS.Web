using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewScore
    {
        public int ReviewHeaderId { get; set; }
        public int AppraiserId { get; set; }
        public int ReviewMetricTypeId { get; set; }
        public decimal TotalScore { get; set; }
    }
}
