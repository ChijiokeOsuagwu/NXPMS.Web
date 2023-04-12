using Microsoft.AspNetCore.Mvc.Rendering;
using NXPMS.Base.Models.SecurityModels;
using NXPMS.Web.Models.UtilityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NXPMS.Web.Models.SecurityModels
{
    public class UserListViewModel:BaseViewModel
    {
        public List<ApplicationUser> UserList { get; set; }
    }
}
