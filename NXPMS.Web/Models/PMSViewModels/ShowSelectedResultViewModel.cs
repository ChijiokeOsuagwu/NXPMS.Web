using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ShowSelectedResultViewModel:BaseViewModel
    {
        public int id { get; set; }
        public int ad { get; set; }
        public EvaluationResultViewModel EvaluationSummaryResult { get; set; }
        public EvaluationListViewModel KpaFullResult { get; set; }
        public EvaluationListViewModel CmpFullResult { get; set; }
        public ReviewHeader ReviewHeaderInfo { get; set; }
        public  List<ReviewCDG> ReviewCDGs { get; set; }
    }
}
