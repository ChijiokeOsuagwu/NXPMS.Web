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
    public class ManageEmployeeViewModel
    {
        public int? EmployeeID { get; set; }
        public string EmployeeNo { get; set; }

        [Display(Name = "Custom Code")]
        [MaxLength(20, ErrorMessage = "Custom Code must not exceed 20 characters.")]
        public string CustomNo { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [Display(Name = "Last Name*")]
        [MaxLength(60, ErrorMessage = "Last Name must not exceed 60 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [Display(Name = "First Name*")]
        [MaxLength(60, ErrorMessage = "First Name must not exceed 60 characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Other Names")]
        [MaxLength(60, ErrorMessage = "Other Names must not exceed 60 characters.")]
        public string OtherNames { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [Display(Name = "Gender*")]
        public string Sex { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(50, ErrorMessage = "Phone Number must not exceed 50 characters.")]
        public string PhoneNo { get; set; }

        [Display(Name = "Alt. Phone Number")]
        [MaxLength(50, ErrorMessage = "Alt. Phone Number must not exceed 50 characters.")]
        public string AltPhoneNo { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile ImageUpload { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Range(1, 31)]
        public int? BirthDay { get; set; }

        [Range(1, 12)]
        public int? BirthMonth { get; set; }

        public int? BirthYear { get; set; }

        [Display(Name = "Birth Day")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Start Up Date")]
        [DataType(DataType.Date)]
        public DateTime? StartUpDate { get; set; }

        [Display(Name = "Job Grade")]
        public string JobGrade { get; set; }

        [Display(Name = "Start Up Designation")]
        [MaxLength(100, ErrorMessage = "Start Up Designation must not exceed 50 characters.")]
        public string StartUpDesignation { get; set; }

        [Display(Name = "Employment Status")]
        public EmploymentStatus EmployeeStatus { get; set; }

        [Display(Name = "Residence Address")]
        [MaxLength(350, ErrorMessage = "Residence Address must not exceed 350 characters.")]
        [DataType(DataType.MultilineText)]
        public string ResidenceAddress { get; set; }

        [Display(Name = "Permanent Address")]
        [MaxLength(350, ErrorMessage = "Residence Address must not exceed 350 characters.")]
        [DataType(DataType.MultilineText)]
        public string PermanentHomeAddress { get; set; }

        [Display(Name = "Official Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Official Email must not exceed 250 characters.")]
        public string OfficialEmail { get; set; }

        [Display(Name = "Personal Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Personal Email must not exceed 250 characters.")]
        public string PersonalEmail { get; set; }


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

        [Display(Name="Category")]
        public string EmployeeCategoryDescription { get; set; }

        [Display(Name = "Type")]
        public int? EmployeeTypeID { get; set; }

        [Display(Name = "Type")]
        public string EmployeeTypeDescription { get; set; }

        [Display(Name="Job Profile")]
        public int? JobProfileID { get; set; }

        [Display(Name="Job Profile")]
        public string JobProfileName { get; set; }
        
        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "LGA of Origin")]
        [MaxLength(100, ErrorMessage = "LGA of Origin must not exceed 50 characters.")]
        public string LgaOfOrigin { get; set; }

        [Display(Name = "Date of Last Promotion")]
        [DataType(DataType.Date)]
        public DateTime? DateOfLastPromotion { get; set; }

        [Display(Name = "Confirmation Date")]
        [DataType(DataType.Date)]
        public DateTime? ConfirmationDate { get; set; }

        [Display(Name = "Current Designation")]
        [MaxLength(100, ErrorMessage = "Current Designation must not exceed 250 characters.")]
        public string CurrentDesignation { get; set; }

        [Display(Name = "Years of Experience")]
        public int? YearsOfExperience { get; set; }

        [Display(Name = "Religion")]
        public string Religion { get; set; }

        [Display(Name = "Geo-Political Region")]
        public string GeoPoliticalRegion { get; set; }
        internal string ImagePath { get; set; }

        public Employee ConvertToEmployee() => new Employee
        {
            AltPhoneNo = AltPhoneNo,
            BirthDay = BirthDay ?? 0,
            BirthMonth = BirthMonth ?? 0,
            BirthYear = BirthYear ?? 0,
            ConfirmationDate = ConfirmationDate,
            CurrentDesignation = CurrentDesignation,
            CustomNo = CustomNo,
            DepartmentCode = DepartmentCode,
            DepartmentName = DepartmentName,
            EmployeeCategoryDescription = EmployeeCategoryDescription,
            EmployeeCategoryID = EmployeeCategoryID ?? 0,
            EmployeeID = EmployeeID ?? 0,
            EmployeeNo = EmployeeNo,
            EmployeeStatus = EmployeeStatus,
            EmployeeTypeDescription = EmployeeTypeDescription,
            EmployeeTypeID = EmployeeTypeID ?? 0,
            FirstName = FirstName,
            FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
            GeoPoliticalRegion = GeoPoliticalRegion,
            ImagePath =  ImagePath,
            JobProfileID = JobProfileID,
            JobProfileName = JobProfileName,
            LocalGovernmentOfOrigin = LgaOfOrigin,
            LocationID = LocationID ?? 0,
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
            Surname = Surname,
            StartUpDate = StartUpDate,
            StartUpDesignation = StartUpDesignation,
            StateOfOrigin = StateOfOrigin,
            Title = Title,
            UnitCode = UnitCode,
            UnitName = UnitName,
            YearsOfExperience = YearsOfExperience ?? 0,
        };

        public ManageEmployeeViewModel ExtractFromEmployee(Employee employee) =>
             new ManageEmployeeViewModel
            {
                AltPhoneNo = employee.AltPhoneNo,
                BirthDay = employee.BirthDay,
                BirthMonth = employee.BirthMonth,
                BirthYear = employee.BirthYear,
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
                FirstName = employee.FirstName,
                FullName = employee.FullName,
                GeoPoliticalRegion = employee.GeoPoliticalRegion,
                ImagePath = employee.ImagePath,
                JobProfileID = employee.JobProfileID,
                JobProfileName = employee.JobProfileName,
                LgaOfOrigin = employee.LocalGovernmentOfOrigin,
                LocationID = employee.LocationID,
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
                Surname = employee.Surname,
                StartUpDate = employee.StartUpDate,
                StartUpDesignation = employee.StartUpDesignation,
                StateOfOrigin = employee.StateOfOrigin,
                Title = employee.Title,
                UnitCode = employee.UnitCode,
                UnitName = employee.UnitName,
                YearsOfExperience = employee.YearsOfExperience,
            };
    }
}
