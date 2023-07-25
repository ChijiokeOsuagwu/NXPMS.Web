using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AppraisalApprovalViewModel:BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int ReviewSessionID { get; set; }
        public int AppraiseeID { get; set; }
        public int ReviewStageID { get; set; }
        public List<ReviewApproval> ReviewApprovalList { get; set; }

    }
}
