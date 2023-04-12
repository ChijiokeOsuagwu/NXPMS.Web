using NXPMS.Base.Enums;
using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeEmploymentInfoViewModel : BaseViewModel
    {
        public int? EmployeeID { get; set; }
        public string EmployeeNo { get; set; }

        [Display(Name = "Custom Code")]
        [MaxLength(20, ErrorMessage = "Custom Code must not exceed 20 characters.")]
        public string CustomNo { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Official Email")]
        [DataType(DataType.EmailAddress)]
        public string OfficialEmail { get; set; }

        [Display(Name = "Start Up Date")]
        [DataType(DataType.Date)]
        public DateTime? StartUpDate { get; set; }

        [Display(Name = "Start Up Designation")]
        [MaxLength(100, ErrorMessage = "Start Up Designation must not exceed 100 characters.")]
        public string StartUpDesignation { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage="Status is required.")]
        public EmploymentStatus EmployeeStatus { get; set; }

        [Display(Name = "Location*")]
        [Required(ErrorMessage = "Location is required.")]
        public int? LocationID { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit*")]
        public string UnitCode { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Place of Engagement")]
        [MaxLength(100, ErrorMessage = "Place of Engagement must not exceed 50 characters.")]
        public string PlaceOfEngagement { get; set; }

        [Display(Name = "Category")]
        public int? EmployeeCategoryID { get; set; }

        [Display(Name = "Category")]
        public string EmployeeCategoryDescription { get; set; }

        [Display(Name = "Type")]
        public int? EmployeeTypeID { get; set; }

        [Display(Name = "Type")]
        public string EmployeeTypeDescription { get; set; }

        [Display(Name = "Job Profile")]
        public int? JobProfileID { get; set; }

        [Display(Name = "Job Profile")]
        public string JobProfileName { get; set; }

        [Display(Name = "Confirmation Date")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationDate { get; set; }

        [Display(Name = "Current Designation")]
        [MaxLength(100, ErrorMessage = "Current Designation must not exceed 250 characters.")]
        public string CurrentDesignation { get; set; }

        public Employee ConvertToEmployee() =>
             new Employee {
                ConfirmationDate = ConfirmationDate,
                CurrentDesignation = CurrentDesignation,
                CustomNo = CustomNo,
                DepartmentCode = DepartmentCode,
                DepartmentName = DepartmentName,
                EmployeeCategoryDescription = EmployeeCategoryDescription,
                EmployeeCategoryID = EmployeeCategoryID,
                EmployeeID = EmployeeID.Value,
                EmployeeNo = EmployeeNo,
                EmployeeStatus = EmployeeStatus,
                EmployeeTypeDescription = EmployeeTypeDescription,
                EmployeeTypeID = EmployeeTypeID,
                FullName = FullName,
                JobProfileID = JobProfileID,
                JobProfileName =  JobProfileName,
                LocationID = LocationID,
                LocationName = LocationName,
                OfficialEmail = OfficialEmail,
                PlaceOfEngagement = PlaceOfEngagement,
                StartUpDate = StartUpDate,
                StartUpDesignation = StartUpDesignation,
                UnitCode = UnitCode,
                UnitName = UnitName,
             };

        public EmployeeEmploymentInfoViewModel ExtractFromEmployee(Employee employee) =>
             new EmployeeEmploymentInfoViewModel
             {
                 ConfirmationDate = employee.ConfirmationDate,
                 CurrentDesignation = employee.CurrentDesignation,
                 CustomNo = employee.CustomNo,
                 DepartmentCode = employee.DepartmentCode,
                 DepartmentName = employee.DepartmentName,
                 EmployeeCategoryDescription = employee.EmployeeCategoryDescription,
                 EmployeeCategoryID = employee.EmployeeCategoryID,
                 EmployeeID = employee.EmployeeID,
                 EmployeeNo = employee.EmployeeNo,
                 EmployeeStatus = employee.EmployeeStatus,
                 EmployeeTypeDescription = employee.EmployeeTypeDescription,
                 EmployeeTypeID = employee.EmployeeTypeID,
                 FullName = employee.FullName,
                 JobProfileID = employee.JobProfileID,
                 JobProfileName = employee.JobProfileName,
                 LocationID = employee.LocationID,
                 LocationName = employee.LocationName,
                 OfficialEmail = employee.OfficialEmail,
                 PlaceOfEngagement = employee.PlaceOfEngagement,
                 StartUpDate = employee.StartUpDate,
                 StartUpDesignation = employee.StartUpDesignation,
                 UnitCode = employee.UnitCode,
                 UnitName = employee.UnitName,
             };
    }


}
