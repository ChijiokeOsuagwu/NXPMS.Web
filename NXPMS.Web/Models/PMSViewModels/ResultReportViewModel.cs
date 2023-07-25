using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ResultReportViewModel:BaseViewModel
    {
        public int id { get; set; }
        public int? lc { get; set; }
        public string dc { get; set; }
        public string uc { get; set; }
        public string ReviewSessionDescription { get; set; }
        public List<ResultDetail> ResultDetailList { get; set; }
    }
}
