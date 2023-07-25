using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ApprovalRole
    {
        public int ApprovalRoleId { get; set; }
        public string ApprovalRoleName { get; set; }
        public bool MustApproveContract { get; set; }
        public bool MustApproveEvaluation { get; set; }
    }
}
