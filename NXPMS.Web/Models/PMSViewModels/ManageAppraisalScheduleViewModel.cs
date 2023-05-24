using NXPMS.Base.Enums;
using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ManageAppraisalScheduleViewModel:BaseViewModel
    {
        public int SessionScheduleId { get; set; }

        [Required]
        public int ReviewSessionId { get; set; }
        public string ReviewSessionName { get; set; }
        public int ReviewYearId { get; set; }
        public string ReviewYearName { get; set; }

        [Required]
        public int ScheduleTypeId { get; set; }

        [Display(Name="Schedule Type")]
        public string ScheduleTypeDescription { get; set; }

        [Required]
        [Display(Name="Activity Type")]
        public int ActivityTypeId { get; set; }

        [Display(Name="Acivity Type Description")]
        public string ActivityTypeDescription { get; set; }

        [Display(Name = "Location")]
        public int? ScheduleLocationId { get; set; }

        [Display(Name = "Location")]
        public string ScheduleLocationName { get; set; }

        [Display(Name = "Department")]
        public string ScheduleDepartmentCode { get; set; }

        [Display(Name = "Department")]
        public string ScheduleDepartmentName { get; set; }

        [Display(Name = "Unit")]
        public string ScheduleUnitCode { get; set; }

        [Display(Name = "Unit")]
        public string ScheduleUnitName { get; set; }

        [Display(Name = "Employee")]
        public int? ScheduleEmployeeId { get; set; }

        [Display(Name = "Employee")]
        public string ScheduleEmployeeName { get; set; }

        [Required]
        [Display(Name="Start From")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduleStartTime { get; set; }

        [Required]
        [Display(Name="End On")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduleEndTime { get; set; }
     
        public SessionSchedule ConvertToSessionSchedule()
        {
            return new SessionSchedule
            {
                ActivityType = (SessionActivityType)ActivityTypeId,
                SessionActivityTypeDescription = ActivityTypeDescription,
                ReviewSessionId = ReviewSessionId,
                ReviewSessionName = ReviewSessionName,
                ReviewYearId = ReviewYearId,
                ReviewYearName = ReviewYearName,
                ScheduleDepartmentCode = ScheduleDepartmentCode,
                ScheduleDepartmentName = ScheduleDepartmentName,
                ScheduleEmployeeId = ScheduleEmployeeId,
                ScheduleEmployeeName = ScheduleEmployeeName,
                ScheduleEndTime = ScheduleEndTime,
                ScheduleLocationId = ScheduleLocationId,
                ScheduleLocationName = ScheduleLocationName,
                ScheduleStartTime = ScheduleStartTime,
                ScheduleType = (SessionScheduleType)ScheduleTypeId,
                ScheduleTypeDescription = ScheduleTypeDescription,
                ScheduleUnitCode = ScheduleUnitCode,
                ScheduleUnitName = ScheduleUnitName,
                SessionScheduleId = SessionScheduleId,
            };
        }
    }
}
