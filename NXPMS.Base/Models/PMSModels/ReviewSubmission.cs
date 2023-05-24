﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class ReviewSubmission
    {
        public int ReviewSubmissionId { get; set; }
        public int ReviewHeaderId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int FromEmployeeId { get; set; }
        public string FromEmployeeName { get; set; }
        public int ToEmployeeId { get; set; }
        public string ToEmployeeName { get; set; }
        public int ToEmployeeRoleId { get; set; }
        public string ToEmployeeRoleName { get; set; }
        public int SubmissionPurposeId { get; set; }
        public string SubmissionPurposeDescription { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        public string SubmissionMessage { get; set; }
        public bool IsActioned { get; set; }
        public DateTime? TimeActioned { get; set; }
    }
}
