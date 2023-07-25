using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class MyAppraisalStepsViewModel:BaseViewModel
    {
        public List<ReviewStage> AppraisalStageList { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewSessionId { get; set; }
        public int AppraiseeId { get; set; }
        public int? ReviewHeaderId { get; set; }
        public int CurrentReviewStageId { get; set; }
        public string AppraiseeName { get; set; }
        public bool IsActive { get; set; }
        public bool AllActivitiesScheduled { get; set; }
        public bool ContractDefinitionScheduled { get; set; }
        public bool PerformanceEvaluationScheduled { get; set; }
    }
}
