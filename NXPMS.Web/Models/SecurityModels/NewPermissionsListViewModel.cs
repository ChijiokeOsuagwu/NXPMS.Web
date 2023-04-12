using NXPMS.Base.Models.SecurityModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SecurityModels
{
    public class NewPermissionsListViewModel:BaseViewModel
    {
        public string pd { get; set; }
        public int UserID { get; set; }
        public List<ApplicationRole> ApplicationRolesList { get; set; }
    }
}
