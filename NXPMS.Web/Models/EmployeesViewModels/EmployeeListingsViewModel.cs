using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeListingsViewModel:BaseViewModel
    {
        public int ld { get; set; }
        public string dc { get; set; }
        public string uc { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
