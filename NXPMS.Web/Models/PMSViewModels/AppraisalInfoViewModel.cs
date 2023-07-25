using NXPMS.Base.Models.PMSModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AppraisalInfoViewModel
    {
        public int ReviewSubmissionID { get; set; }
        public ReviewHeader AppraisalReviewHeader { get; set; }
        public List<ReviewMetric> CompetencyList { get; set; }
        public List<ReviewMetric> KpaList { get; set; }
        public List<ReviewCDG> CdgList { get; set; }
    }
}
