using NXPMS.Base.Models.GlobalSettingsModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SettingsModels
{
    public class DepartmentListViewModel:BaseViewModel
    {
        public List<Department> DepartmentList { get; set; }
    }
}
