using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.EmployeesModels
{
    public class EmployeeReport
    {
        public int EmployeeReportId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int ReportsToId { get; set; }
        public string ReportsToName { get; set; }
        public string ReportsToDesignation { get; set; }
        public string ReportsToUnitCode { get; set; }
        public string ReportsToUnitName { get; set; }
        public string ReportsToDepartmentCode { get; set; }
        public string ReportsToDepartmentName { get; set; }
        public int? ReportsToLocationId { get; set; }
        public string ReportsToLocationName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
