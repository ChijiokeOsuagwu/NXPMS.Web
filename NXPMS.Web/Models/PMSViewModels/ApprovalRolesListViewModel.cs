using NXPMS.Base.Models.PMSModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.PMSViewModels
{
    public class ApprovalRolesListViewModel:BaseViewModel
    {
        public List<ApprovalRole> ApprovalRoleList { get; set; }
    }
}
