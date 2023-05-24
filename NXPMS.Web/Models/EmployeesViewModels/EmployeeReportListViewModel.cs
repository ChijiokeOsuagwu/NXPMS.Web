using NXPMS.Base.Models.EmployeesModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeReportListViewModel:BaseViewModel
    {
        public int ID { get; set; }
        public List<EmployeeReport> EmployeeReportList { get; set; }
    }
}
