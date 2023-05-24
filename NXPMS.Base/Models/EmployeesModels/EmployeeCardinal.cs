using System;
using System.Collections.Generic;
using System.Text;

namespace NXPMS.Base.Models.EmployeesModels
{
    public class EmployeeCardinal
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeUnitCode { get; set; }
        public string EmployeeDepartmentCode { get; set; }
        public int EmployeeLocationId { get; set; } 
    }
}
