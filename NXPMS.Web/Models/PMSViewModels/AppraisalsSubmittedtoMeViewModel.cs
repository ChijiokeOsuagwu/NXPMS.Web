using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AppraisalsSubmittedtoMeViewModel:BaseViewModel
    {
        public int AppraiserID { get; set; }
        public string AppraiserName { get; set; }
        public int? id { get; set; }
        public List<ReviewSubmission> ReviewSubmissionList { get; set; }
    }
}
