using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeSearchViewModel:BaseViewModel
    {
        public string est { get; set; }
        public List<Employee> EmployeesList { get; set; }
    }
}
