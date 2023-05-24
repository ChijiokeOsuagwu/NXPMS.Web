using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class AppraisalGradeSettingsListViewModel:BaseViewModel
    {
        public int ReviewSessionID { get; set; }
        public List<AppraisalGrade> AppraisalPerformanceGradeList { get; set; }
        public List<AppraisalGrade> AppraisalCompetencyGradeList { get; set; }
    }
}
