﻿using NXPMS.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.PMSModels
{
    public class SessionSchedule
    {
        public int SessionScheduleId { get; set; }
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }
        public SessionScheduleType ScheduleType { get; set; }
        public string ScheduleTypeDescription { get; set; }
        public SessionActivityType ActivityType { get; set; }
        public string SessionActivityTypeDescription { get; set; }
        public int? ScheduleLocationId { get; set; }
        public string ScheduleLocationName { get; set; }
        public string ScheduleDepartmentCode { get; set; }
        public string ScheduleDepartmentName { get; set; }
        public string ScheduleUnitCode { get; set; }
        public string ScheduleUnitName { get; set; }
        public int? ScheduleEmployeeId { get; set; }
        public string ScheduleEmployeeName { get; set; }
        public DateTime? ScheduleStartTime { get; set; }
        public DateTime? ScheduleEndTime { get; set; }
        public string ScheduleStatus { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime? CancelledTime { get; set; }
        public string CancelledBy { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
