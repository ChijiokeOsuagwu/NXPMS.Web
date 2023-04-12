using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.EmployeesModels
{
    public class EmployeeType
    {
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypeName { get; set; }
        public int EmployeeCategoryId { get; set; }
        public string EmployeeCategoryName { get; set; }
    }
}
