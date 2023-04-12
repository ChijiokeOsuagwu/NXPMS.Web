using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeePersonalInfoViewModel:BaseViewModel
    {
        public int? EmployeeID { get; set; }

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

        [Display(Name = "Personal Email")]
        [MaxLength(250, ErrorMessage = "Personal Email Address must not exceed 250 characters.")]
        [DataType(DataType.EmailAddress)]
        public string PersonalEmail { get; set; }

        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }

        [Range(1, 31)]
        public int? BirthDay { get; set; }

        [Range(1, 12)]
        public int? BirthMonth { get; set; }

        [Range(1920, 2080)]
        public int? BirthYear { get; set; }

        [Display(Name = "Date of Birth")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Residence Address")]
        [MaxLength(350, ErrorMessage = "Residence Address must not exceed 350 characters.")]
        [DataType(DataType.MultilineText)]
        public string ResidenceAddress { get; set; }

        [Display(Name = "Permanent Address")]
        [MaxLength(350, ErrorMessage = "Residence Address must not exceed 350 characters.")]
        [DataType(DataType.MultilineText)]
        public string PermanentHomeAddress { get; set; }

        [Display(Name = "State of Origin")]
        public string StateOfOrigin { get; set; }

        [Display(Name = "LGA of Origin")]
        [MaxLength(100, ErrorMessage = "LGA of Origin must not exceed 50 characters.")]
        public string LgaOfOrigin { get; set; }

        [Display(Name = "Religion")]
        public string Religion { get; set; }

        [Display(Name = "Geo-Political Region")]
        public string GeoPoliticalRegion { get; set; }

        public string ImagePath { get; set; }

        public Employee ConvertToEmployee() => new Employee
        {
            AltPhoneNo = AltPhoneNo,
            BirthDay = BirthDay ?? 0,
            BirthMonth = BirthMonth ?? 0,
            BirthYear = BirthYear ?? 0,
            EmployeeID = EmployeeID ?? 0,
            FirstName = FirstName,
            FullName = $"{Title} {FirstName} {OtherNames} {Surname}",
            GeoPoliticalRegion = GeoPoliticalRegion,
            ImagePath = ImagePath,
            LocalGovernmentOfOrigin = LgaOfOrigin,
            MaritalStatus = MaritalStatus,
            OtherNames = OtherNames,
            PermanentHomeAddress = PermanentHomeAddress,
            PersonalEmail = PersonalEmail,
            PhoneNo = PhoneNo,
            Religion = Religion,
            ResidenceAddress = ResidenceAddress,
            Sex = Sex,
            Surname = Surname,
            StateOfOrigin = StateOfOrigin,
            Title = Title,
        };

        public EmployeePersonalInfoViewModel ExtractFromEmployee(Employee employee) =>
             new EmployeePersonalInfoViewModel
             {
                 AltPhoneNo = employee.AltPhoneNo,
                 BirthDay = employee.BirthDay,
                 BirthMonth = employee.BirthMonth,
                 BirthYear = employee.BirthYear,
                 EmployeeID = employee.EmployeeID,
                 FirstName = employee.FirstName,
                 FullName = employee.FullName,
                 GeoPoliticalRegion = employee.GeoPoliticalRegion,
                 ImagePath = employee.ImagePath,
                 LgaOfOrigin = employee.LocalGovernmentOfOrigin,
                 MaritalStatus = employee.MaritalStatus,
                 OtherNames = employee.OtherNames,
                 PermanentHomeAddress = employee.PermanentHomeAddress,
                 PersonalEmail = employee.PersonalEmail,
                 PhoneNo = employee.PhoneNo,
                 Religion = employee.Religion,
                 ResidenceAddress = employee.ResidenceAddress,
                 Sex = employee.Sex,
                 Surname = employee.Surname,
                 StateOfOrigin = employee.StateOfOrigin,
                 Title = employee.Title,
             };
    }
}
