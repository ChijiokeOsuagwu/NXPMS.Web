using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewSchedule
    {
        public bool AllActivitiesScheduled { get; set; }
        public bool ContractDefinitionScheduled { get; set; }
        public bool PerformanceEvaluationScheduled { get; set; }
    }
}
