using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class MoveToNextStageModel
    {
        public int ReviewHeaderID { get; set; }
        public int AppraiseeID { get; set; }
        public int PrincipalAppraiserID { get; set; }
        public int ReviewSessionID { get; set; }
        public int CurrentStageID { get; set; }
        public int NextStageID { get; set; }
        public bool IsQualifiedToMove { get; set; }
        public string NextStageDescription { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<string> WarningMessages { get; set; }
    }
}
