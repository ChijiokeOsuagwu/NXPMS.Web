using Microsoft.AspNetCore.Http;
using NXPMS.Base.Enums;
using NXPMS.Base.Models.EmployeesModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }

        [Display(Name = "Category")]
        public int EmployeeCategoryID { get; set; }
        [Display(Name = "Category")]
        public string EmployeeCategoryDescription { get; set; }

        [Display(Name = "Type")]
        public int EmployeeTypeID { get; set; }

        [Display(Name = "Type")]
        public string EmployeeTypeDescription { get; set; }

        [Display(Name = "Title")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Display(Name = "Last Name")]
        [MaxLength(60)]
        [Required]
        public string Surname { get; set; }

        [Display(Name = "First Name")]
        [MaxLength(60)]
        [Required]
        public string FirstName { get; set; }

        [Display(Name = "Other Names")]
        [MaxLength(60)]
        public string OtherNames { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Gender")]
        public string Sex { get; set; }

        [Display(Name = "Phone No.")]
        [MaxLength(60)]
        public string PhoneNo { get; set; }

        [Display(Name = "Alt Phone No.")]
        [MaxLength(60)]
        public string AltPhoneNo { get; set; }

        [Display(Name = "Personal Email")]
        [MaxLength(160)]
        [DataType(DataType.EmailAddress)]
        public string PersonalEmail { get; set; }

        [Display(Name = "Official Email")]
        [MaxLength(160)]
        [DataType(DataType.EmailAddress)]
        public string OfficialEmail { get; set; }

        [Display(Name = "Residence Address")]
        [MaxLength(250)]
        [DataType(DataType.MultilineText)]
        public string ResidenceAddress { get; set; }

        [Display(Name = "Permanent Home Address")]
        [MaxLength(250)]
        [DataType(DataType.MultilineText)]
        public string PermanentHomeAddress { get; set; }

        public string ImagePath { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile ImageUpload { get; set; }

        [Display(Name = "Birth Day")]
        public int BirthDay { get; set; }

        [Display(Name = "Birth Month")]
        public int BirthMonth { get; set; }

        [Display(Name = "Birth Year")]
        public int BirthYear { get; set; }

        [Display(Name = "Employee No")]
        [MaxLength(20)]
        public string EmployeeNo { get; set; }

        [Display(Name = "Custom No")]
        [MaxLength(20)]
        [DataType(DataType.MultilineText)]
        public string CustomNo { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartUpDate { get; set; }

        [Display(Name = "Start Designation")]
        [MaxLength(100)]
        public string StartUpDesignation { get; set; }

        [Display(Name = "Place of Engagement")]
        [MaxLength(100)]
        [DataType(DataType.MultilineText)]
        public string PlaceOfEngagement { get; set; }

        [Display(Name = "State of Origin")]
        [MaxLength(50)]
        public string StateOfOrigin { get; set; }

        [Display(Name = "Local Government of Origin")]
        [MaxLength(100)]
        public string LocalGovernmentOfOrigin { get; set; }

        [Display(Name = "Religion")]
        [MaxLength(50)]
        public string Religion { get; set; }

        [Display(Name = "Geo-Political Zone")]
        [MaxLength(60)]
        public string GeoPoliticalRegion { get; set; }

        [Display(Name = "Location")]
        public int LocationID { get; set; }

        [Display(Name = "Location")]
        public string LocationName { get; set; }

        [Display(Name = "Department")]
        public string DepartmentCode { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Unit")]
        public string UnitCode { get; set; }

        [Display(Name = "Unit")]
        public string UnitName { get; set; }

        [Display(Name = "Status")]
        public EmploymentStatus EmployeeStatus { get; set; }

        [Display(Name = "Marital Status")]
        [MaxLength(30)]
        public string MaritalStatus { get; set; }

        public Employee ConvertToEmployee()
        {
            return new Employee { 
            AltPhoneNo = AltPhoneNo,
            BirthDay = BirthDay,
            BirthMonth = BirthMonth,
            BirthYear = BirthYear,
            CustomNo = CustomNo,
            DepartmentCode = DepartmentCode,
            DepartmentName = DepartmentName,
            EmployeeCategoryDescription = EmployeeCategoryDescription,
            EmployeeCategoryID = EmployeeCategoryID,
            EmployeeID = EmployeeID,
            EmployeeNo = EmployeeNo,
            EmployeeStatus = EmployeeStatus,
            EmployeeTypeDescription = EmployeeTypeDescription,
            EmployeeTypeID = EmployeeTypeID,
            FirstName = FirstName,
            FullName = FullName,
            GeoPoliticalRegion = GeoPoliticalRegion,
            ImagePath = ImagePath,
            LocalGovernmentOfOrigin = LocalGovernmentOfOrigin,
            LocationID = LocationID,
            LocationName = LocationName,
            MaritalStatus = MaritalStatus,
            OfficialEmail = OfficialEmail,
            OtherNames = OtherNames,
            PermanentHomeAddress = PermanentHomeAddress,
            PersonalEmail = PersonalEmail,
            PhoneNo = PhoneNo,
            PlaceOfEngagement = PlaceOfEngagement,
            Religion = Religion,
            ResidenceAddress = ResidenceAddress,
            Sex = Sex,
            StartUpDate = StartUpDate,
            StartUpDesignation = StartUpDesignation,
            StateOfOrigin = StateOfOrigin,
            Surname = Surname,
            Title = Title,
            UnitCode = UnitCode,
            UnitName = UnitName,
            };
        }
    
        public EmployeeViewModel ExtractFromEmployee(Employee employee)
        {
            return new EmployeeViewModel
            {
                AltPhoneNo = employee.AltPhoneNo,
                BirthDay = employee.BirthDay,
                BirthMonth = employee.BirthMonth,
                BirthYear = employee.BirthYear,
                CustomNo = employee.CustomNo,
                DepartmentCode = employee.DepartmentCode,
                DepartmentName = employee.DepartmentName,
                EmployeeCategoryDescription = employee.EmployeeCategoryDescription,
                EmployeeCategoryID = employee.EmployeeCategoryID ?? 0,
                EmployeeID = employee.EmployeeID,
                EmployeeNo = employee.EmployeeNo,
                EmployeeStatus = employee.EmployeeStatus,
                EmployeeTypeDescription = employee.EmployeeTypeDescription,
                EmployeeTypeID = employee.EmployeeTypeID ?? 0,
                FirstName = employee.FirstName,
                FullName = employee.FullName,
                GeoPoliticalRegion = employee.GeoPoliticalRegion,
                ImagePath = employee.ImagePath,
                LocalGovernmentOfOrigin = employee.LocalGovernmentOfOrigin,
                LocationID = employee.LocationID ?? 0,
                LocationName = employee.LocationName,
                MaritalStatus = employee.MaritalStatus,
                OfficialEmail = employee.OfficialEmail,
                OtherNames = employee.OtherNames,
                PermanentHomeAddress = employee.PermanentHomeAddress,
                PersonalEmail = employee.PersonalEmail,
                PhoneNo = employee.PhoneNo,
                PlaceOfEngagement = employee.PlaceOfEngagement,
                Religion = employee.Religion,
                ResidenceAddress = employee.ResidenceAddress,
                Sex = employee.Sex,
                StartUpDate = employee.StartUpDate,
                StartUpDesignation = employee.StartUpDesignation,
                StateOfOrigin = employee.StateOfOrigin,
                Surname = employee.Surname,
                Title = employee.Title,
                UnitCode = employee.UnitCode,
                UnitName = employee.UnitName,
            };
        }
    }
}
