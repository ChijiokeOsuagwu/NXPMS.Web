using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class EvaluationListViewModel:BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public int AppraiseeID { get; set; }
        public int SubmissionID { get; set; }
        public int AppraiserID { get; set; }
        public List<ReviewResult> ReviewResultList { get; set; }
    }
}
