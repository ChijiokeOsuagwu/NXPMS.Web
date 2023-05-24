using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeReportLineViewModel : BaseViewModel
    {
        public int EmployeeReportId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int ReportsToId { get; set; }

        [Required]
        [Display(Name = "Reports To:")]
        public string ReportsToName { get; set; }

        [Display(Name = "Starting From: ")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ending On: ")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        public EmployeeReport ConvertToEmployeeReport()
        {
            return new EmployeeReport
            {
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeName,
                EmployeeReportId = EmployeeReportId,
                EndDate = EndDate,
                IsCurrent = IsCurrent,
                ReportsToId = ReportsToId,
                ReportsToName = ReportsToName,
                StartDate = StartDate,
            };
}
        public EmployeeReportLineViewModel ExtractToModel(EmployeeReport employeeReport)
        {
            return new EmployeeReportLineViewModel
            {
                EmployeeId = employeeReport.EmployeeId,
                EmployeeName = employeeReport.EmployeeName,
                EmployeeReportId = employeeReport.EmployeeReportId,
                EndDate = employeeReport.EndDate,
                IsCurrent = employeeReport.IsCurrent,
                ReportsToId = employeeReport.ReportsToId,
                ReportsToName = employeeReport.ReportsToName,
                StartDate = employeeReport.StartDate,
            };
        }
    }
}
