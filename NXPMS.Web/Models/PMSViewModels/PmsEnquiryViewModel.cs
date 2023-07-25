using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class PmsEnquiryViewModel:BaseViewModel
    {
        public string nm { get; set; }
        public int id { get; set; }
        public string dc { get; set; }
        public string uc { get; set; }
        public List<ResultSummary> ResultSummaryList { get; set; }
    }
}
