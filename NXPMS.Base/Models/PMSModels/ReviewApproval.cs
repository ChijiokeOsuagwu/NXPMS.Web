﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewApproval
    {
        public int ReviewApprovalId { get; set; }
        public int ApprovalTypeId { get; set; }
        public string ApprovalTypeDescription { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ApproverId { get; set; }
        public string ApproverName { get; set; }
        public int ApproverRoleId { get; set; }
        public string ApproverRoleDescription { get; set; }
        public bool MustApproveContract { get; set; }
        public bool MustApproveEvaluation { get; set; }
        public int? AppraiseeId { get; set; }
        public string AppraiseeName { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedTime { get; set; }
        public string ApprovedComments { get; set; }
        public int? ReviewMetricId { get; set; }
        public string ReviewMetricDescription { get; set; }
        public int SubmissionPurposeId { get; set; }
    }
}
