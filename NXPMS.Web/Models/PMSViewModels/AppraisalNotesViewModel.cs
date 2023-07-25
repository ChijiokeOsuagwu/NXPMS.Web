using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AppraisalNotesViewModel:BaseViewModel
    {
        public int ReviewHeaderID { get; set; }
        public int LoggedInEmployeeID { get; set; }
        public int ReviewSessionID { get; set; }
        public int AppraiseeID { get; set; }
        public string AppraiseeName { get; set; }
        public string SourcePage { get; set; }
        public List<ReviewMessage> ReviewMessageList { get; set; }
    }
}
