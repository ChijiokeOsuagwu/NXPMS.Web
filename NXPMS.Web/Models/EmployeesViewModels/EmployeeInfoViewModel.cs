using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.EmployeesViewModels
{
    public class EmployeeInfoViewModel:BaseViewModel
    {
        public EmployeePersonalInfoViewModel PersonalInfo { get; set; }
        public EmployeeEmploymentInfoViewModel EmploymentInfo { get; set; }
        public EmployeeNextOfKinInfoViewModel NextOfKinInfo { get; set; }
    }
}
