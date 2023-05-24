using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ReviewGradeListViewModel : BaseViewModel
    {
        public int? TemplateId { get; set; }
        public int? SessionId { get; set; }
        public List<ReviewGrade> ReviewGradeList{ get;set; }
    }
}
