using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class DirectReportEvaluationsViewModel:BaseViewModel
    {
        public int sd { get; set; }
        public int id { get; set; } 
        public List<ResultSummary> ReportsResultSummaryList { get; set; }
    }
}
