using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewCDG
    {
        public int ReviewCdgId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public int AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public int ReviewHeaderId { get; set; }
        public string ReviewCdgDescription { get; set; }
        public string ReviewCdgObjective { get; set; }
        public string ReviewCdgActionPlan { get; set; }
    }
}
