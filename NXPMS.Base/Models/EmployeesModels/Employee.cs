using NXPMS.Base.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.EmployeesModels
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public int? EmployeeCategoryID { get; set; }
        public string EmployeeCategoryDescription { get; set; }
        public int? EmployeeTypeID { get; set; }
        public string EmployeeTypeDescription { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherNames { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string PhoneNo { get; set; }
        public string AltPhoneNo { get; set; }
        public string PersonalEmail { get; set; }
        public string OfficialEmail { get; set; }
        public string ResidenceAddress { get; set; }
        public string PermanentHomeAddress { get; set; }
        public string ImagePath { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public string EmployeeNo { get; set; }
        public string CustomNo { get; set; }
        public DateTime? StartUpDate { get; set; }
        public string StartUpDesignation { get; set; }
        public string PlaceOfEngagement { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public string CurrentDesignation { get; set; }
        public int? JobProfileID { get; set; }
        public string JobProfileName { get; set; }
        public string StateOfOrigin { get; set; }
        public string LocalGovernmentOfOrigin { get; set; }
        public string Religion { get; set; }
        public string GeoPoliticalRegion { get; set; }
        public string NextOfKinName { get; set; }
        public string NextOfKinRelationship { get; set; }
        public string NextOfKinAddress { get; set; }
        public string NextOfKinPhoneNo { get; set; }
        public string NextOfKinEmail { get; set; }
        public int? LocationID { get; set; }
        public string LocationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public int YearsOfExperience { get; set; }
        public EmploymentStatus EmployeeStatus { get; set; }
        public string MaritalStatus { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
