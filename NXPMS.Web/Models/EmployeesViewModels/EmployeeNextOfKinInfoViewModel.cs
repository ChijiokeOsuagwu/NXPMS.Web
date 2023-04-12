using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeNextOfKinInfoViewModel:BaseViewModel
    {
        public int? EmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string FullName { get; set; }

        [Display(Name = "Name*")]
        [MaxLength(150, ErrorMessage="Name must not exceed 150 characters.")]
        public string NextOfKinName { get; set; }

        [Display(Name = "Relationship*")]
        public string NextOfKinRelationship { get; set; }

        [Display(Name = "Address")]
        [MaxLength(250, ErrorMessage = "Address must not exceed 250 characters.")]
        public string NextOfKinAddress { get; set; }

        [Display(Name = "Phone No")]
        [MaxLength(30, ErrorMessage = "Phone Number must not exceed 30 characters.")]
        public string NextOfKinPhoneNo { get; set; }

        [Display(Name = "Email")]
        [MaxLength(250, ErrorMessage = "Email must not exceed 250 characters.")]
        public string NextOfKinEmail { get; set; }


        public Employee ConvertToEmployee() =>
      new Employee
      {

          EmployeeID = EmployeeID.Value,
          FullName = FullName,
          NextOfKinAddress = NextOfKinAddress,
          NextOfKinEmail = NextOfKinEmail,
          NextOfKinName = NextOfKinName,
          NextOfKinPhoneNo = NextOfKinPhoneNo,
          NextOfKinRelationship = NextOfKinRelationship
      };

        public EmployeeNextOfKinInfoViewModel ExtractFromEmployee(Employee employee) =>
             new EmployeeNextOfKinInfoViewModel
             {
                 EmployeeID = employee.EmployeeID,
                 FullName = employee.FullName,
                 NextOfKinAddress = employee.NextOfKinAddress,
                 NextOfKinEmail = employee.NextOfKinEmail,
                 NextOfKinName = employee.NextOfKinName,
                 NextOfKinPhoneNo = employee.NextOfKinPhoneNo,
                 NextOfKinRelationship = employee.NextOfKinRelationship
             };

    }
}
